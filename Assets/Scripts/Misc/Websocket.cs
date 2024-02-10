using System;
using System.Text;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;
using UnityEngine.Networking;

namespace Misc
{
    public static class Websocket
    {
        private static TokenExchangeResponse _token;

        public static async UniTask GetToken(string url)
        {
            {
                await Post(url,
                    (unityWebRequest) =>
                    {
                        unityWebRequest.SetRequestHeader("keyId", "2f217d4c-c87d-4a67-ad28-fdfb27d14912");
                        unityWebRequest.SetRequestHeader("keySecret", "YzoG2M57BcL53QbLkHg9r-rv7g26UWVk");
                        unityWebRequest.SetRequestHeader("projectId", "6f0cda60-b504-4ba5-b315-6614687db17d");
                        unityWebRequest.SetRequestHeader("environmentId", "ff22f140-ed50-4e70-a7a1-9012898c751c");
                        unityWebRequest.SetRequestHeader("fleetId", "dfef6f52-a5be-45c4-875d-083f5e7be9f9");
                        // unityWebRequest.SetRequestHeader("buildConfigurationId", "1253762");
                        // unityWebRequest.SetRequestHeader("regionId", "e8854343-ae07-4c85-adeb-f0a1edc407b2");
                    },
                    "",
                    (string error) => { Debug.Log("Error: " + error); },
                    (string json) =>
                    {
                        Debug.Log(json);
                        _token = JsonUtility.FromJson<TokenExchangeResponse>(json);
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
                (UnityWebRequest unityWebRequest) =>
                {
                    unityWebRequest.SetRequestHeader("Authorization", "Bearer " + _token.accessToken);
                    unityWebRequest.SetRequestHeader("keyId", "2f217d4c-c87d-4a67-ad28-fdfb27d14912");
                    unityWebRequest.SetRequestHeader("keySecret", "YzoG2M57BcL53QbLkHg9r-rv7g26UWVk");
                    unityWebRequest.SetRequestHeader("projectId", "6f0cda60-b504-4ba5-b315-6614687db17d");
                    unityWebRequest.SetRequestHeader("environmentId", "ff22f140-ed50-4e70-a7a1-9012898c751c");
                    unityWebRequest.SetRequestHeader("fleetId", "dfef6f52-a5be-45c4-875d-083f5e7be9f9");
                    unityWebRequest.SetRequestHeader("buildConfigurationId", "1253762");
                    unityWebRequest.SetRequestHeader("regionId", "e8854343-ae07-4c85-adeb-f0a1edc407b2");
                }, ""
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
                }
            );
            await UniTask.Delay(1000);
            LobbyManager.Instance.JoinLobbyAllocationId(allocationId, callback).Forget();
        }
    }
}