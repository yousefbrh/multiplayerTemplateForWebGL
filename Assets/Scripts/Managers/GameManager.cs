using System;
using System.Threading.Tasks;
using Components;
using Misc;
using Network.Leaderboard;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        #region SerializeField
        #endregion

        #region Private
        #endregion

        #region Public
        public static GameManager Instance;
        public Action OnGameStart;
        public Action OnGameStartPreview;
        public Action OnGameStop;
        public Action OnGameResume;
        public Action OnGameReset;
        [HideInInspector] public bool isStopped;
        [HideInInspector] public bool isStarted;
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
                Destroy(gameObject);
                return;
            }

            Register();
            SetGameState(false);
        }

        private void Start()
        {
            SubscribeListeners();
            LocalAdManager.Instance.Init();
        }

        private void Register()
        {
            Utility.CreateId();
        }
        
        private void SetGameState(bool isActive)
        {
            isStopped = !isActive;
            isStarted = isActive;
        }

        private void SubscribeListeners()
        {
            
        }

        public async void PlayerNameChanged(string value)
        {
            await UpdatePlayerNameAsync(value);
            GlobalLeaderboard.Instance.UpdateLeaderboard();
        }
        
        private async Task UpdatePlayerNameAsync(string value)
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(value);
        }
        
        public async void IsAuthenticated(bool isActive)
        {
            if (isActive)
            {
                var playerName = Prefs.GetPlayerName();
                if (playerName != string.Empty)
                {
                    await UpdatePlayerNameAsync(playerName);
                    GlobalLeaderboard.Instance.UpdateLeaderboard();
                }
            }
            UIManager.Instance.Authenticated(isActive);

        }
        
        public void CreateGame()
        {
            LobbyManager.Instance.CreateGame(RespondCallback);
            UIManager.Instance.ApplyCreateGameSetting();
        }
        
        private void RespondCallback(bool isSuccessful)
        {
            if (!isSuccessful)
                CancelMatchMade(false);
            UIManager.Instance.ApplyGameRespond(isSuccessful);
        }

        public void CancelMatchMade(bool isUpdateLeaderboard)
        {
            UIManager.Instance.ApplyCancelingGame();
            if (isUpdateLeaderboard)
                GlobalLeaderboard.Instance.UpdateLeaderboard();
        }
        
        private void StartTheGamePreview()
        {
            SetGameState(true);
            OnGameStartPreview?.Invoke();
        }
        
        public void StopTheGame()
        {
            Time.timeScale = 0;
            SetGameState(false);
            OnGameStop?.Invoke();
        }
        
        public void ResumeTheGame()
        {
            Time.timeScale = 1;
            SetGameState(true);
            OnGameResume?.Invoke();
        }

        public void SetSceneActionSettings(bool isSet)
        {
            if (isSet)
                NetworkManager.Singleton.SceneManager.OnLoadComplete += SceneManagerOnOnLoadComplete;
            else
                NetworkManager.Singleton.SceneManager.OnLoadComplete -= SceneManagerOnOnLoadComplete;
        }
        
        private void SceneManagerOnOnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            UIManager.Instance.ShowEntryPanel(false);
        }
    }
}
