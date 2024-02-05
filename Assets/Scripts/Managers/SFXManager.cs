using Misc;
using UnityEngine;

namespace Managers
{
    public class SFXManager : MonoBehaviour
    {
        #region SerializeField
        #endregion

        #region Private
        #endregion

        #region Public
        public static SFXManager instance;
        public GameObject audioSourcePrefab;
        public AudioClip[] audioClips;
        [HideInInspector] public bool isEnabled;
        #endregion
        
        private void Awake()
        {
            if (instance)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                instance = this;
            }
            isEnabled = Utility.IsSfxEnable();
        }

        public void Play(int index)
        {
            if (!isEnabled) return;
        
            AudioSource audioSource = Instantiate(audioSourcePrefab, transform).GetComponent<AudioSource>();
            audioSource.clip = audioClips[index];
            audioSource.Play();
            Destroy(audioSource.gameObject, audioSource.clip.length);
        }

        public void SetActive(bool value)
        {
            isEnabled = value;
            Utility.SetSfxActive(value);
        }
    }
}