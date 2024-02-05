using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Enums;
using Managers;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Random = System.Random;

namespace Misc
{
    public static class Utility
    {
        private const string PlayerIdKey = "PlayerFullId";
        private const string PlayerNameKey = "PlayerName";
        private const string MusicKey = "MusicEnabled";
        private const string SfxKey = "SoundEffectsEnabled";
        private const string FirstShowPanelKey = "isFirstShownCharacterPanel";
        private const string SkinIndexKey = "CurrentSkinIndex";
        private static TokenExchangeResponse _token;
        
        public const string EntrySceneName = "Entry";
        public static readonly int AddHashKey = Animator.StringToHash("Add");
        public static readonly int RemoveHashKey = Animator.StringToHash("Remove");

        public static IEnumerator CheckReachability(Action<bool> action)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get("https://opensheet.elk.sh/1U7QsW86isvPeQqzwQHNkGFKDGohtGftCuXx4EFLIjlo/Test");
            yield return webRequest.SendWebRequest();
            action?.Invoke(string.IsNullOrEmpty(webRequest.error));
        }
        
        public static void CreateId()
        {
            if (PlayerPrefs.HasKey(PlayerIdKey)) return;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 16)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray()
            );
            PlayerPrefs.SetString("PlayerFullId", result);
        }

        public static bool HasName()
        {
            var bridgeName = BridgeDataManager.Instance.GetPlayerName();
            var hasName = bridgeName != "";
            if (!hasName)
                hasName = PlayerPrefs.HasKey("PlayerName");
            return hasName;
        }

        public static string GetPlayerName(string defaultValue = "")
        {
            return PlayerPrefs.GetString("PlayerName", defaultValue);
        }

        public static void SetPlayerName(string value)
        {
            PlayerPrefs.SetString(PlayerNameKey, value);
        }

        public static bool IsMusicEnable(int defaultValue = 1)
        {
            return PlayerPrefs.GetInt(MusicKey, defaultValue) == 1;
        }

        public static void SetMusicActive(bool value)
        {
            PlayerPrefs.SetInt(MusicKey, value ? 1 : 0);
        }

        public static bool IsSfxEnable(int defaultValue = 1)
        {
            return PlayerPrefs.GetInt(SfxKey, defaultValue) == 1;
        }
        
        public static void SetSfxActive(bool value)
        {
            PlayerPrefs.SetInt(SfxKey, value ? 1 : 0);
        }

        public static int GetPoints(int defaultValue = 0)
        {
            return PlayerPrefs.GetInt("Point", defaultValue);
        }
        
        public static void SetPoints(int value)
        { 
            PlayerPrefs.SetInt("Point", value);
        }

        public static bool IsSkinPanelShown(int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(FirstShowPanelKey, defaultValue) == 1;
        }
        
        public static void SetSkinPanelShown(bool value)
        {
            PlayerPrefs.SetInt(FirstShowPanelKey, value ? 1 : 0);
        }

        public static int GetSkinIndex(int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(SkinIndexKey, defaultValue);
        }
        
        public static void SetSkinIndex(int value)
        {
            PlayerPrefs.GetInt(SkinIndexKey, value);
        }
        
        public static async UniTask GetToken(string url)
        {
            {
                await Post(url,
                    (unityWebRequest) => { },
                    "",
                    (string error) => { Debug.Log("Error: " + error); },
                    (string json) =>
                    {
                        _token = JsonUtility.FromJson<TokenExchangeResponse>(json);
                        Debug.Log(_token.AccessToken);
                    });
            }
        }
        
        public static async UniTask Get(string url, Action<UnityWebRequest> setHeaderAction, Action<string> onError, Action<string> onSuccess)
        {
            await WebRequestSand(url, setHeaderAction, onError, onSuccess); 
        }

        public static async UniTask Post(string url, Action<UnityWebRequest> setHeaderAction, string jsonData, Action<string> onError, Action<string> onSuccess)
        {
            await WebRequestPostJson(url, setHeaderAction, jsonData, onError, onSuccess);
        }

        private static async UniTask WebRequestSand(string url, Action<UnityWebRequest> setHeaderAction, Action<string> onError, Action<string> onSuccess)
        {
            using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(url))
            {
                if (setHeaderAction != null)
                {
                    setHeaderAction(unityWebRequest); 
                }

                await unityWebRequest.SendWebRequest().ToUniTask();

                if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError ||
                    unityWebRequest.result == UnityWebRequest.Result.DataProcessingError ||
                    unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    // Error
                    onError(unityWebRequest.error);
                }
                else
                {
                    onSuccess(unityWebRequest.downloadHandler.text);
                }
            }
        }

        private static async UniTask WebRequestPostJson(string url, Action<UnityWebRequest> setHeaderAction, string jsonData, Action<string> onError, Action<string> onSuccess)
        {
            using (UnityWebRequest unityWebRequest = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                unityWebRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                unityWebRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                unityWebRequest.SetRequestHeader("Content-Type", "application/json");

                if (setHeaderAction != null)
                {
                    setHeaderAction(unityWebRequest);
                }

                await unityWebRequest.SendWebRequest().ToUniTask();

                if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError ||
                    unityWebRequest.result == UnityWebRequest.Result.DataProcessingError ||
                    unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    // Error
                    onError(unityWebRequest.error);
                }
                else
                {
                    onSuccess(unityWebRequest.downloadHandler.text);
                }
            }
        }
        
        public static async UniTask InitCreateServerButton(Action<bool> callback, string url)
        {
            var allocationId = "";
            await Post(url,
                (UnityWebRequest unityWebRequest) => { unityWebRequest.SetRequestHeader("Authorization", "Bearer " + _token.AccessToken); }, ""
                ,
                (string error) =>
                {
                    Debug.Log("Error: " + error);
                    callback?.Invoke(false);
                },
                (string json) =>
                {
                    allocationId = json;
                    Debug.Log(allocationId);
                    //      Debug.Log("Success: " + allocationId);
                }
            );
            await UniTask.Delay(1000);
            LobbyManager.Instance.JoinLobbyAllocationId(allocationId, callback).Forget();
        }
        
        public static IEnumerator LoadScene(string sceneName, float delay, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            yield return new WaitForSeconds(delay);
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, loadSceneMode);
        }
    }
}