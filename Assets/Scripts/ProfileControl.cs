using UnityEngine;
using UnityEngine.UI;

public class ProfileControl : SingletonBehaviour<ProfileControl>
{
    private const string NAME_PREFIX = "NAME_";
    private const string PROGRESS_PREFIX = "PROGRESS_";
    
    [SerializeField] InputField _playerNameInputField;

    private ProgressData _progressData;
    public ProgressData ProgressData { get => _progressData; }

    private string _enteredPlayerName;
    private string _loadedPlayerPref;
    private string _jsonText;

    protected override void Initialize()
    {
        dontDestroyOnload = true;
    }

    public void SubmitPlayerName()
    {
        _enteredPlayerName = _playerNameInputField.text;
        if (PlayerNameExists(_enteredPlayerName))
        {
            _jsonText = PlayerPrefs.GetString(PROGRESS_PREFIX + _enteredPlayerName);
            _progressData = JsonUtility.FromJson<ProgressData>(_jsonText);
        }
        else
        {
            _progressData = new ProgressData(_enteredPlayerName);
            SaveProgressData();
        }
    }

    public void SaveProgressData()
    {
        _jsonText = JsonUtility.ToJson(_progressData);
        PlayerPrefs.SetString(NAME_PREFIX + _enteredPlayerName, _enteredPlayerName);
        PlayerPrefs.SetString(PROGRESS_PREFIX + _enteredPlayerName, _jsonText);
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
