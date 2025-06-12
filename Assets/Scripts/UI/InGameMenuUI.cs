using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenuUI : SingletonBehaviour<InGameMenuUI>
{
    [Header("Scene navigation settings references")]
    [SerializeField] private string _restartConfirmationWindowText = "Restart Current Level?";
    [SerializeField] private string _goToMainMenuConfirmationWindowText = "Go to Main Menu?";
    [SerializeField] private Transform _sceneNavigationBackgroundPanel;
    [SerializeField] private Button _sceneNavigationButton;

    [Header("Sound Settings references")]
    [SerializeField] private Slider _effectsSoundSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Transform _soundSettingsBackgroundPanel;
    [SerializeField] private Button _settingsButton;

    [Header("Current Level information")]
    [SerializeField] private Button _currentLevelInformationButton;

    public Action<float> EffectsSoundVolumeChanged;
    public Action<float> MusicVolumeChanged;

    public float EffectsSoundVolume { get => _effectsSoundVolume; }
    private float _effectsSoundVolume;
    public float MusicVolume { get => _musicVolume; }
    private float _musicVolume;
    private ScenesManagementSO _scenesManagementSO;

    protected override void Initialize()
    {
        dontDestroyOnload = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        _scenesManagementSO = SceneManagement.Instance.ScenesManagementSO;
        _effectsSoundVolume = _effectsSoundSlider.value;
        _musicVolume = _musicVolumeSlider.value;
        // check if current scene is main scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (_scenesManagementSO.MainSceneBuildIndex == currentSceneIndex)
        {
            _settingsButton.gameObject.SetActive(true);
            //_sceneNavigationButton.gameObject.SetActive(false);
            //_currentLevelInformationButton.gameObject.SetActive(false);
        }
        else
        {
            _settingsButton.gameObject.SetActive(true);
            _sceneNavigationButton.gameObject.SetActive(true);
            _currentLevelInformationButton.gameObject.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void RestartButtonOnClick()
    {
        GeneralInformationWindowUI.Instance.CallWindow(_restartConfirmationWindowText, GeneralInformationWindowUI.InformationType.CONFIRMATION, RestartScene);
    }

    public void GoToMainMenuButtonOnClick()
    {
        GeneralInformationWindowUI.Instance.CallWindow(_goToMainMenuConfirmationWindowText, GeneralInformationWindowUI.InformationType.CONFIRMATION, LoadMainMenu);
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void OnEffectsSoundChange()
    {
        _effectsSoundVolume = _effectsSoundSlider.value;
        EffectsSoundVolumeChanged?.Invoke(_effectsSoundVolume);
    }

    public void OnMusicVolumeChange()
    {
        _musicVolume = _musicVolumeSlider.value;
        MusicVolumeChanged?.Invoke(_musicVolume);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _soundSettingsBackgroundPanel.gameObject.SetActive(false);
        _sceneNavigationBackgroundPanel.gameObject.SetActive(false);
    }
}
