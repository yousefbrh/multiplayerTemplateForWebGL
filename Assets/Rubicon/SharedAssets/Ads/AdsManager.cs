using System;
using UnityEngine;

namespace Ads
{
    public class AdsManager : MonoBehaviour
    {
        public static AdsManager instance;
        public event Action RewardSuccess, RewardFailed;
        public event Action InterSuccess, InterFailed;
        public bool InterReady => _adapter?.InterReady ?? false;
        public bool RewardReady => _adapter?.RewardReady ?? false;
        
        [SerializeField] private GameObject _adapterObject;
        [Header("Inter Delayer")]
        
        [SerializeField] private bool _interShowDelayer = false;
        [SerializeField] private float _delayBeforeInter = 5f;
        private readonly string _delayerPath = "Rubicon/AdBreak";
        private InterDelayer _interDelayer;
        private IAdsAdapter _adapter;
        private Action _rewardSuccessAction, _rewardFailedAction, _interAction;

        private void Awake()
        {
            if (!instance)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            _adapterObject.TryGetComponent(out _adapter);
            if (_adapter == null)
                return;
            _adapter.RewardSuccess += OnRewardSuccess;
            _adapter.RewardFailed += OnRewardFailed;
            _adapter.InterSuccess += OnInterSuccess;
            _adapter.InterFailed += OnInterFailed;
            if (_interShowDelayer)
            {
                InitInterDelayer();
            }
        }

        private void InitInterDelayer()
        {
            _interDelayer = Instantiate(Resources.Load<InterDelayer>(_delayerPath), transform);
            _interDelayer.Init();
        }

        private void OnInterFailed()
        {
            _interAction?.Invoke();
            InterFailed?.Invoke();
        }

        private void OnInterSuccess()
        {
            _interAction?.Invoke();
            InterSuccess?.Invoke();
        }

        private void OnRewardFailed()
        {
            _rewardFailedAction?.Invoke();
            RewardFailed?.Invoke();
        }

        private void OnRewardSuccess()
        {
            _rewardSuccessAction?.Invoke();
            RewardSuccess?.Invoke();
        }

        public void ShowReward(Action success = null, Action failed = null)
        {
            _rewardSuccessAction = success;
            _rewardFailedAction = failed;
#if UNITY_EDITOR
            OnRewardSuccess();
            return;
#endif
            _adapter.ShowReward();
        }
        
        public void ShowInter(Action success = null)
        {
            _interAction = success;

#if UNITY_EDITOR
            if (_interShowDelayer)
            {
                _interDelayer.StartInter(OnInterSuccess, _delayBeforeInter);
                return;
            }
            OnInterSuccess();
            return;
#endif
            if (_interShowDelayer)
            {
                _interDelayer.StartInter(_adapter.ShowInter, _delayBeforeInter);
                return;
            }
            _adapter.ShowInter();
         }
    }
}