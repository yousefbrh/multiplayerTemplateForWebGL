using System;
using DG.Tweening;
using Misc;
using UnityEngine;

namespace Managers
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager instance;

        [SerializeField] private AudioSource audioSource;

        [Space]
    
        [SerializeField] private float fadeInDuration = 1;
        [SerializeField] private float fadeOutDuration = 0.5f;

        [HideInInspector] public bool isEnabled;

        private float defaultVolume;

        private void Awake()
        {
            if (instance)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                instance = this;
            }

            defaultVolume = audioSource.volume;

            isEnabled = Utility.IsMusicEnable();
            if (isEnabled) FadeIn(fadeInDuration);
        }

        private void Play()
        {
            if (audioSource.time > 0) audioSource.UnPause();
            else audioSource.Play();
        }

        private void Stop() => audioSource.Pause();

        public void FadeIn(float duration, Action onComplete = null)
        {
            audioSource.DOFade(defaultVolume, duration)
                .OnStart(Play)
                .onComplete += () => onComplete?.Invoke();
        }

        public void FadeOut(float duration, Action onComplete = null)
        {
            audioSource.DOFade(0, duration)
                .onComplete += () =>
            {
                Stop();
                onComplete?.Invoke();
            };
        }

        public void SetActive(bool value)
        {
            switch (isEnabled)
            {
                case true when value == false:
                    FadeOut(fadeOutDuration);
                    break;
                case false when value == true:
                    FadeIn(fadeInDuration);
                    break;
            }
        
            isEnabled = value;
            Utility.SetMusicActive(value);
        }
    }
}