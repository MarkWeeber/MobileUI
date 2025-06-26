using System.Collections.Generic;
using UnityEngine;

public class ProfileControl : SingletonBehaviour<ProfileControl>
{
    private const string NAME_PREFIX = "NAME_";
    private const string PROGRESS_PREFIX = "PROGRESS_";
    
    private ProgressData _progressData;
    public ProgressData ProgressData { get => _progressData; }

    private string _loadedPlayerPref;
    private string _jsonText;

    protected override void Initialize()
    {
        dontDestroyOnload = true;
    }

    public void SubmitPlayerName(string playerName)
    {
        if (PlayerNameExists(playerName))
        {
            _jsonText = PlayerPrefs.GetString(PROGRESS_PREFIX + playerName);
            _progressData = JsonUtility.FromJson<ProgressData>(_jsonText);
        }
        else
        {
            _progressData = new ProgressData(playerName);
            SaveProgressData();
        }
    }

    public void SubmitScore(int levelId, int score)
    {
        int index = _progressData.ProgressMetrics.FindIndex(n => n.LevelId == levelId);
        ProgressMetric progressMetric = new ProgressMetric();
        // metric alread exists
        if (index >= 0)
        {
            progressMetric = _progressData.ProgressMetrics[index];
            progressMetric.LastScore = score;
            if (progressMetric.MaxScore < score)
            {
                progressMetric.MaxScore = score;
            }
            _progressData.ProgressMetrics[index] = progressMetric;
        }
        // a new metric
        else
        {
            progressMetric.LevelId = levelId;
            progressMetric.MaxScore = score;
            progressMetric.LastScore = score;
            _progressData.ProgressMetrics.Add(progressMetric);
        }
        SaveProgressData();
    }

    public Score[] GetScoresPerLevel(int levelId, bool sorted = true)
    {
        var scores = new Score[1];
        return scores;
    }

    private void SaveProgressData()
    {
        _jsonText = JsonUtility.ToJson(_progressData);
        PlayerPrefs.SetString(NAME_PREFIX + _progressData.ProfileName, _progressData.ProfileName);
        PlayerPrefs.SetString(PROGRESS_PREFIX + _progressData.ProfileName, _jsonText);
    }

    private bool PlayerNameExists(string enteredPlayerName)
    {
        _loadedPlayerPref = PlayerPrefs.GetString(NAME_PREFIX + enteredPlayerName);
        if (_loadedPlayerPref != string.Empty)
        {
            return true;
        }
        else
        {
            return false;
        }    
    }

}

public struct Score
{
    public string PlayerName;
    public int LastScore;
    public int BestScore;
}
