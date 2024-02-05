using System;
using Misc;
using UnityEngine;

namespace Managers
{
    public class LocalAdManager : MonoBehaviour
    {
        #region SerializeField
        [SerializeField] private float showAdTime;
        #endregion

        #region Private
        private bool _canShowAd;
        private bool _interstitialStarted;
        private bool _canCountDown;
        private float _tempTime;
        #endregion

        #region Public
        public static LocalAdManager Instance;
        public Action<bool> onShowAdCalled;
        #endregion
        
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Instance.CheckToShowAd();
                Destroy(gameObject);
                return;
            }
        }

        public void Init()
        {
            _canCountDown = true;
        }
        
        private void Update()
        {
            HandleCountDown();
        }

        private void HandleCountDown()
        {
            if (!_canCountDown) return;
            _tempTime += Time.deltaTime;
            if (!(_tempTime >= showAdTime)) return;
            _canCountDown = false;
            _canShowAd = true;
            Debug.Log("AdReady");
        }

        private void CheckToShowAd()
        {
            if (!_canShowAd) return;
            onShowAdCalled?.Invoke(false);
            ShowAd();
            _canCountDown = true;
            _canShowAd = false;
            _tempTime = 0;
        }
        
        private void ShowAd()
        {
            if (_interstitialStarted) return;

            _interstitialStarted = true;

            StartCoroutine(Utility.CheckReachability((isReachable) =>
            {
                if (!isReachable)
                {
                    _interstitialStarted = false;
                    return;
                }

#if !UNITY_EDITOR
            AdsManager.instance.ShowInter();
#endif
            }));
        }
        
    }
}