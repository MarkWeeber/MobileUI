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
            SaveProgressData(playerName);
        }
    }

    public void SaveProgressData(string enteredPlayerName)
    {
        _jsonText = JsonUtility.ToJson(_progressData);
        PlayerPrefs.SetString(NAME_PREFIX + enteredPlayerName, enteredPlayerName);
        PlayerPrefs.SetString(PROGRESS_PREFIX + enteredPlayerName, _jsonText);
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
