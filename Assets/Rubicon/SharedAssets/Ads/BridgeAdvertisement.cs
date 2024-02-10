using System;
using System.Runtime.InteropServices;
using Ads;
using UnityEngine;

namespace Rubicon.Ads
{
    public class BridgeAdvertisement : MonoBehaviour, IAdsAdapter
    {
        public bool InterReady { get; private set; }
        public bool RewardReady { get; private set; }
        public RewardedState CurrentRewardedState { get; private set; }
        public InterstitialState CurrentInterstitialState { get; private set; }
        public event Action RewardSuccess, RewardFailed, InterSuccess, InterFailed;
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern void InstantGamesBridgeShowRewarded();
        [DllImport("__Internal")] private static extern void InstantGamesBridgeShowInterstitial();
#endif
        public void InterstitialStateChanged(string value)
        {
            if (Enum.TryParse(value, true, out InterstitialState state))
            {
                CurrentInterstitialState = state;
                switch (CurrentInterstitialState)
                {
                    case InterstitialState.Opened:
                        gameObject.SendMessage("AdsOpen");
                        break;
                    case InterstitialState.Closed:
                        gameObject.SendMessage("AdsClose");
                        InterSuccess?.Invoke();
                        break;
                    case InterstitialState.Failed:
                        InterFailed?.Invoke();
                        break;
                }
            }
            print("### Inter: " + CurrentInterstitialState);
        }

        public void RewardedStateChanged(string value)
        {
            if (Enum.TryParse(value, true, out RewardedState state))
            {
                CurrentRewardedState = state;
                switch (state)
                {
                    case RewardedState.Opened:
                        gameObject.SendMessage("AdsOpen");
                        break;
                    case RewardedState.Closed:
                        gameObject.SendMessage("AdsClose");
                        break;
                    case RewardedState.Rewarded:
                        RewardSuccess?.Invoke();
                        break;
                    case RewardedState.Failed: 
                        RewardFailed?.Invoke();
                        break;
                }
            }
            print("### Reward: " + CurrentRewardedState);
        }

        

        public void ShowReward()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            InstantGamesBridgeShowRewarded();
#endif
        }

        public void ShowInter()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            InstantGamesBridgeShowInterstitial();
#endif
        }

        public void SetPlatform(string platformName)
        {
            print("BridgeAdvertisement.SetPlatform: " + platformName);
            if (platformName.ToLower() == "mock")
                return;
            InterReady = true;
            RewardReady = true;
        }
    }

    public enum RewardedState
    {
        Loading,
        Opened,
        Rewarded,
        Closed,
        Failed
    }

    public enum InterstitialState
    {
        Loading,
        Opened,
        Closed,
        Failed
    }
}