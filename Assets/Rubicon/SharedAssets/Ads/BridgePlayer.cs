using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rubicon.Ads
{
    public class BridgePlayer : MonoBehaviour
    {
        private Action<string> _platformIdAction, _platformLangAction, _platformDeviceTypeAction, _playerNameAction;
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern string InstantGamesBridgeGetPlatformId();
        [DllImport("__Internal")] private static extern string InstantGamesBridgeGetPlatformLanguage();
        [DllImport("__Internal")] private static extern string InstantGamesBridgeGetDeviceType();
        [DllImport("__Internal")] private static extern string InstantGamesBridgePlayerName();
        [DllImport("__Internal")] private static extern string InstantGamesBridgePlayerAvatar();
#endif

        //request
        public void GetPlatformId(Action<string> action)
        {
            print("### Game GetPlatformId");
            _platformIdAction = action;
#if UNITY_WEBGL && !UNITY_EDITOR
            action?.Invoke(InstantGamesBridgeGetPlatformId());
#else
            action?.Invoke("mock");
#endif
        }

        public void GetPlatformLanguage(Action<string> action)
        {
            print("### Game GetPlatformLanguage");
            _platformLangAction = action;
#if UNITY_WEBGL && !UNITY_EDITOR
            action?.Invoke(InstantGamesBridgeGetPlatformLanguage());
#else
            action?.Invoke("en");
#endif
        }

        public void GetDeviceType(Action<string> action)
        {
            print("### Game GetDeviceType");
            _platformDeviceTypeAction = action;
#if UNITY_WEBGL && !UNITY_EDITOR
            action?.Invoke(InstantGamesBridgeGetDeviceType());
#else
            action?.Invoke("desktop");
#endif
        }

        public void GetPlayerName(Action<string> action)
        {
            print("### Game GetPlayerName");
            _playerNameAction = action;
#if UNITY_WEBGL && !UNITY_EDITOR
            action?.Invoke(InstantGamesBridgePlayerName());
#else
            action.Invoke("player_" + Random.Range(0, 1000));
#endif
        }
        
        public void GetPlayerAvatar(Action<string> action)
        {
            print("### Game GetPlayerName");
            _playerNameAction = action;
#if UNITY_WEBGL && !UNITY_EDITOR
            action?.Invoke(InstantGamesBridgePlayerAvatar());
#else
            action.Invoke("player_" + Random.Range(0, 1000));
#endif
        }

        //response
        public void OnGetPlatformId(string platform) => _platformIdAction?.Invoke(platform);
        public void OnGetPlatformLanguage(string lang) => _platformLangAction?.Invoke(lang);
        public void OnGetDeviceType(string deviceType) => _platformDeviceTypeAction?.Invoke(deviceType);
        public void OnGetPlayerName(string playerName) => _playerNameAction?.Invoke(playerName);
    }
}