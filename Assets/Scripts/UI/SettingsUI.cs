using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsUI : SingletonBehaviour<SettingsUI>
{
    [SerializeField] private Slider _effectsSoundSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Transform _backgroundPanel;

    public Action<float> EffectsSoundVolumeChanged;
    public Action<float> MusicVolumeChanged;

    public float EffectsSoundVolume { get => _effectsSoundVolume; }
    private float _effectsSoundVolume;
    public float MusicVolume { get => _musicVolume; }
    private float _musicVolume;

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void Initialize()
    {
        dontDestroyOnload = true;
    }

    private void Start()
    {
        _effectsSoundVolume = _effectsSoundSlider.value;
        _musicVolume = _musicVolumeSlider.value;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
        _backgroundPanel.gameObject.SetActive(false);
    }
}
