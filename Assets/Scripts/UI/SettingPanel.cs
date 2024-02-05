using System;
using Managers;
using Misc;
using TMPro;
using UnityEngine;

namespace UI
{
    public class SettingsPanel : Panel
{
    #region SerializeField
    [Header("Settings Panel: "), Space]
    [SerializeField] private TMP_Text soundEffectStateTMP;
    [SerializeField] private TMP_Text musicStateTMP;
    #endregion

    #region Private
    private GameManager _gameManager;
    #endregion

    #region Public
    public static SettingsPanel Instance;
    #endregion

    private void Awake()
    {
        if (!Instance) Instance = this;
        else return;
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
        Init();
    }

    private void Init()
    {
        soundEffectStateTMP.text = Prefs.IsSfxEnable() ? "ON" : "OFF";

        if (Prefs.IsMusicEnable())
        {
            musicStateTMP.text = "ON";
            MusicManager.instance.FadeIn(3f);
        }
        else musicStateTMP.text = "OFF";
    }

    public override void Show()
    {
        base.Show();
        if (!_gameManager) return;
        if (_gameManager.isStarted)
            _gameManager.StopTheGame();
    }

    public override void Hide()
    {
        base.Hide();

        if (!_gameManager) return;
        if (_gameManager.isStarted)
            _gameManager.ResumeTheGame();
    }

    public void OnSoundEffectsButtonClicked()
    {
        bool isEnabled = !SFXManager.instance.isEnabled;
        
        SFXManager.instance.isEnabled = isEnabled;
        
        if (isEnabled)
        {
            PlayerPrefs.SetInt("SoundEffectsEnabled", 1);
            soundEffectStateTMP.text = "ON";
        }
        else
        {
            PlayerPrefs.SetInt("SoundEffectsEnabled", 0);
            soundEffectStateTMP.text = "OFF";
        }
    }
    
    public void OnMusicButtonClicked()
    {
        bool isEnabled = !MusicManager.instance.isEnabled;
        
        MusicManager.instance.isEnabled = isEnabled;
        
        if (isEnabled)
        {
            PlayerPrefs.SetInt("MusicEnabled", 1);
            MusicManager.instance.FadeIn(1f);
            musicStateTMP.text = "ON";
        }
        else
        {
            PlayerPrefs.SetInt("MusicEnabled", 0);
            MusicManager.instance.FadeOut(0.2f);
            musicStateTMP.text = "OFF";
        }
    }
}
}