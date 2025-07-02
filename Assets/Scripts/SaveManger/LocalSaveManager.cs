using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class LocalSaveManager : SingletonBehaviour<LocalSaveManager>
{
    private const string PREFIX = "_save.dat";
    private const string PASS_PHRASE = "_+$3a0-1!d";
    [SerializeField] private bool _cipheringEnabled = true;

    private ProgressData _progressData;
    public ProgressData ProgressData { get => _progressData; }

    protected override void Initialize()
    {
        dontDestroyOnload = true;
    }

    public async Task SubmitProfileName(string profileName)
    {
        // check if profile already exists
        var (exists, existingProfile) = await ReadProfileTextAsync(profileName);
        if (exists)
        {
            // try parsing it out
            // and if for some reason file corrupted create new save file profile, overwrite the existing one
            if (!TryParseFromJson<ProgressData>(existingProfile, out _progressData))
            {
                _progressData = new ProgressData(profileName);
                await SaveData();
            }
        }
        // if not exists then create new one
        else
        {
            _progressData = new ProgressData(profileName);
            await SaveData();
        }
    }

    public async Task SaveData()
    {
        string fullPath = GetFullPath(_progressData.ProfileName);
        string data = JsonUtility.ToJson(_progressData);
        if (_cipheringEnabled)
        {
            data = StringCipher.Encrypt(data, PASS_PHRASE);
        }
        try
        {
            using (StreamWriter writer = File.CreateText(fullPath))
            {
                await writer.WriteAsync(data);
            }
        }
        catch (Exception)
        {
            Debug.LogError("Could not save locally");
        }
    }

    public void SubmitScore(int levelId, int score)
    {
        int levelIndex = _progressData.ProgressMetrics.FindIndex(n => n.LevelId == levelId);
        ProgressMetric progressMetric = new ProgressMetric();
        // metric already exists
        if (levelIndex >= 0)
        {
            progressMetric = _progressData.ProgressMetrics[levelIndex];
            progressMetric.LastScore = score;
            if (progressMetric.MaxScore < score)
            {
                progressMetric.MaxScore = score;
            }
            _progressData.ProgressMetrics[levelIndex] = progressMetric;
        }
        // a new metric
        else
        {
            progressMetric.LevelId = levelId;
            progressMetric.MaxScore = score;
            progressMetric.LastScore = score;
            _progressData.ProgressMetrics.Add(progressMetric);
        }
        // silent calling async Task
        _ = SaveData();
    }

    /// <summary>
    /// Gets array of scores from all player profiles for specific level ID
    /// </summary>
    /// <returns>
    /// Array of Score, returns default type if no profiles found
    /// </returns>
    public async Task<Score[]> GetScores(int levelId)
    {
        List<ProgressData> allProgressData = await GetProgressDataFromAllProfiles();
        if (allProgressData.Count == 0)
        {
            return default;
        }
        return allProgressData
            .SelectMany(
                        profile => profile.ProgressMetrics
                            .Where(metric => metric.LevelId == levelId)
                            .Select(
                                        metric => new Score
                                        {
                                            PlayerName = profile.ProfileName,
                                            LastScore = metric.LastScore,
                                            BestScore = metric.MaxScore
                                        }
                                    )
                        ).OrderByDescending(score => score.BestScore).ToArray();
    }

    private async Task<List<ProgressData>> GetProgressDataFromAllProfiles()
    {
        List<ProgressData> _result = new List<ProgressData>();
        string[] allFilesInPersisntenFolder = Directory.GetFiles(Application.persistentDataPath);
        string[] matchingFiles = allFilesInPersisntenFolder.Where(file => Path.GetFileName(file).Contains(PREFIX)).ToArray();
        if (matchingFiles.Length == 0)
        {
            Debug.LogWarning("No profile save files found");
            return null;
        }
        foreach (string profileFile in matchingFiles)
        {
            string profileName = profileFile.Replace(PREFIX, "");
            var (exists, existingProfile) = await ReadProfileTextAsync(profileName);
            if (exists)
            {
                if (TryParseFromJson<ProgressData>(existingProfile, out ProgressData progressData))
                {
                    _result.Add(progressData);
                }
            }
        }
        return _result;
    }

    private async Task<(bool, string)> ReadProfileTextAsync(string profileName)
    {
        string assumedFile = GetFullPath(profileName);
        string fileText;
        if (File.Exists(assumedFile))
        {
            using (StreamReader reader = File.OpenText(assumedFile))
            {
                fileText = await reader.ReadToEndAsync();
                return (true, fileText);
            }
        }
        return (false, string.Empty);
    }

    private bool TryParseFromJson<T>(string inputData, out T outputData)
    {
        outputData = default;
        try
        {
            if (_cipheringEnabled)
            {
                string deciphered = StringCipher.Decrypt(inputData, PASS_PHRASE);
                outputData = JsonUtility.FromJson<T>(deciphered);
            }
            else
            {
                outputData = JsonUtility.FromJson<T>(inputData);
            }
            return true;
        }
        catch (Exception)
        {
            Debug.LogWarning("File corrupted, could not read");
            return false;
        }
    }

    private string GetFullPath(string profileName)
    {
        return Application.persistentDataPath + "/" + profileName + PREFIX;
    }
}

public struct Score
{
    public string PlayerName;
    public int LastScore;
    public int BestScore;
}
