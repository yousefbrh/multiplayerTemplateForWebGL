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
    }
}