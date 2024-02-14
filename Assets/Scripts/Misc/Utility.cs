using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Enums;
using Managers;
using Network.Client;
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

        public const string EntrySceneName = "Entry";
        public static readonly int AddHashKey = Animator.StringToHash("Add");
        public static readonly int RemoveHashKey = Animator.StringToHash("Remove");
        public const string HorizontalKey = "Horizontal";
        public const string VerticalKey = "Vertical";

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

        public static IEnumerator LoadScene(string sceneName, float delay, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            yield return new WaitForSeconds(delay);
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, loadSceneMode);
        }
        
        public static void Leave()
        {
            ClientSingleton.Instance.Disconnect();
        }
    }
}