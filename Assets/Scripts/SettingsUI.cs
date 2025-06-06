using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : SingletonBehaviour<SettingsUI>
{
    [SerializeField] private Slider _effectsSoundSlider;
    [SerializeField] private Slider _musicVolumeSlider;

    public Action<float> EffectsSoundVolumeChanged;
    public Action<float> MusicVolumeChanged;

    public float EffectsSoundVolume { get => _effectsSoundVolume; }
    private float _effectsSoundVolume;
    public float MusicVolume { get => _musicVolume; }
    private float _musicVolume;


    protected override void Initialize()
    {
        dontDestroyOnload = true;
    }

    private void Start()
    {
        _effectsSoundVolume = _effectsSoundSlider.value;
        _musicVolume = _musicVolumeSlider.value;
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
}
