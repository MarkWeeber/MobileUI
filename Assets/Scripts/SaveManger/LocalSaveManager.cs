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

    public void SubmitScore(int levelId, float score)
    {
        if (_progressData == null)
        {
            Debug.LogError("Profile not loaded");
            return;
        }
        int levelIndex = _progressData.ProgressMetrics.FindIndex(n => n.LevelId == levelId);
        ProgressMetric progressMetric = new ProgressMetric();
        // metric already exists
        if (levelIndex >= 0)
        {
            progressMetric = _progressData.ProgressMetrics[levelIndex];
            progressMetric.LastTime = score;
            if (progressMetric.BestTime > score)
            {
                progressMetric.BestTime = score;
            }
            _progressData.ProgressMetrics[levelIndex] = progressMetric;
        }
        // add a new metric
        else
        {
            progressMetric.LevelId = levelId;
            progressMetric.BestTime = score;
            progressMetric.LastTime = score;
            _progressData.ProgressMetrics.Add(progressMetric);
        }
        // silent calling async Task to save data
        _ = SaveData();
    }

    public async Task<Score[]> GetScores(int levelId)
    {
        List<ProgressData> allProgressData = await GetProgressDataFromAllProfiles();
        if (allProgressData.Count == 0)
        {
            Debug.LogError("Could not get all profiles");
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
                                            LastTime = metric.LastTime,
                                            BestTime = metric.BestTime
                                        }
                                    )
                        ).OrderByDescending(score => score.BestTime).ToArray();
        // it is important to reverse order, as when score UIs being instantiated, they are placed last in hiearchy by default
    }

    private async Task<List<ProgressData>> GetProgressDataFromAllProfiles()
    {
        List<ProgressData> _result = new List<ProgressData>();
        string[] allFilesInPersisntenFolder = Directory.GetFiles(Application.persistentDataPath);
        string[] matchingFiles = allFilesInPersisntenFolder.Where(file => Path.GetFileName(file).Contains(PREFIX)).ToArray();
        string[] profileFiles = matchingFiles.Select(Path.GetFileName).ToArray();
        if (profileFiles.Length == 0)
        {
            Debug.LogWarning("No profile save files found");
            return _result;
        }
        foreach (string profileFile in profileFiles)
        {
            string profileName = profileFile.Replace(PREFIX, "");
            var (exists, existingProfile) = await ReadProfileTextAsync(profileName);
            if (exists)
            {
                if (TryParseFromJson(existingProfile, out ProgressData progressData))
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
            Debug.LogWarning("Save file corrupted, could not read");
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
    public float LastTime;
    public float BestTime;
}
