using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Misc;
using Network.Client;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Managers
{
    public class LobbyManager : MonoBehaviour
    {
        #region SerializeField
        [SerializeField] private string createServerURL;
        #endregion

        #region Private
        private Lobby _currentLobby;
        #endregion

        #region Public
        public static LobbyManager Instance;
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
        }
        
        public async void CreateGame(Action<bool> callback)
        {
            try
            {
                var options = new QueryLobbiesOptions();
                options.Count = 25;

                options.Filters = new List<QueryFilter>()
                {
                    new QueryFilter(
                        field: QueryFilter.FieldOptions.AvailableSlots,
                        op: QueryFilter.OpOptions.GT,
                        value: "0"),
                    new QueryFilter(
                        field: QueryFilter.FieldOptions.IsLocked,
                        op: QueryFilter.OpOptions.EQ,
                        value: "0")
                };
                var lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);
                if (lobbies.Results.Count == 0)
                {
                    await Websocket.InitCreateServerButton(callback, createServerURL);
                }
                else
                {
                    foreach (var lobby in lobbies.Results)
                    {
                        Debug.Log("lobby counts are : " + lobbies.Results.Count);
                        var canContinue = true;
                        foreach (var player in lobby.Players)
                        {
                            if (player.Data == null) continue;
                            if (player.Data["PlayerFullId"].Value == PlayerPrefs.GetString("PlayerFullId"))
                            {
                                canContinue = false;
                            };
                        }
                        if (!canContinue && lobby.Players.Count == 2) continue;
                        var state = await JoinAsync(lobby, callback);
                        if (!state) continue;
                        _currentLobby = lobby;
                        return;
                    }
                    await Websocket.InitCreateServerButton(callback, createServerURL);
                }
            }
            catch (LobbyServiceException e)
            {
                callback?.Invoke(false);
                Debug.Log(e);
            }
        }
        
        private async Task<bool> JoinAsync(Lobby lobby, Action<bool> callback)
        {
            try
            {
                var joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
                var joinCode = joiningLobby.Data["joinCode"].Value;
                await ClientSingleton.Instance.StartClientAsync(joinCode);
                callback?.Invoke(true);
                return true;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e.Reason);
                Debug.Log(e.ErrorCode);
                Debug.Log(e.Message);
                Debug.Log(e.ApiError);
                return false;
            }
        }
        public async UniTask CreateLobby(string join, string allocationUnityServerId)
        {
            Debug.Log("Create Lobby");
            try
            {
                CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions();
                createLobbyOptions.IsPrivate = false;

                createLobbyOptions.Data = new Dictionary<string, DataObject>()
                {
                    { "joinCode", new DataObject(visibility: DataObject.VisibilityOptions.Public, value: join, DataObject.IndexOptions.S1) },
                    { "id", new DataObject(visibility: DataObject.VisibilityOptions.Public, value: allocationUnityServerId, DataObject.IndexOptions.S2) },
                    { "isStarted", new DataObject(visibility: DataObject.VisibilityOptions.Public, value: "0", DataObject.IndexOptions.N1) },
                    { "name", new DataObject(DataObject.VisibilityOptions.Public, "test", DataObject.IndexOptions.S3) },
                    { "serverPlayerid", new DataObject(DataObject.VisibilityOptions.Public, AuthenticationService.Instance.PlayerId, DataObject.IndexOptions.S4) },
                };
                _currentLobby = await LobbyService.Instance.CreateLobbyAsync("lobby" + AuthenticationService.Instance.PlayerId, 5, createLobbyOptions).AsUniTask();
                SubscribeOnLobby();
                StartCoroutine(HeartbeatLobbyCoroutine(_currentLobby.Id, 29));
                await UniTask.Delay(200);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        private void SubscribeOnLobby()
        {
            try
            {
                var callbacks = new LobbyEventCallbacks();
                callbacks.LobbyChanged += OnLobbyChanged;
                Lobbies.Instance.SubscribeToLobbyEventsAsync(_currentLobby.Id, callbacks);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        private void OnLobbyChanged(ILobbyChanges changes)
        {
            if (changes.LobbyDeleted)
            {
            }
            else
            {
                changes.ApplyToLobby(_currentLobby);
            }
        }
        
        private IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
        {
            var delay = new WaitForSecondsRealtime(waitTimeSeconds);

            while (true)
            {
                LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
                yield return delay;
            }
        }

        public async UniTask JoinLobbyAllocationId(string id, Action<bool> callback)
        {
            try
            {
                QueryLobbiesOptions options = new QueryLobbiesOptions();
                options.Count = 1;
                options.Filters = new List<QueryFilter>()
                {
                    new QueryFilter(QueryFilter.FieldOptions.S2, id, QueryFilter.OpOptions.EQ)
                };
                var isSuccessful = false;
                for (int i = 0; i < 5; i++)
                {
                    await UniTask.Delay(100);
                    var lobbies = await Lobbies.Instance.QueryLobbiesAsync(options).AsUniTask();
                    if (lobbies.Results.Count == 0)
                    {
                        await UniTask.Delay(1000);
                        Debug.Log("Attempt");
                        continue;
                    }
                    callback?.Invoke(true);
                    var playerFullId = PlayerPrefs.GetString("PlayerFullId");
                    var data = new Dictionary<string, PlayerDataObject> { { "PlayerFullId", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerFullId) } };
                    var newPlayer = new Unity.Services.Lobbies.Models.Player()
                    {
                        Data = data
                    };
                    _currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbies.Results[0].Id, new JoinLobbyByIdOptions()
                    {
                        Player = newPlayer
                    });
                    await ClientSingleton.Instance.StartClientAsync(_currentLobby.Data["joinCode"].Value);
                    await UniTask.Delay(2800);
                    isSuccessful = true;
                    break;
                }
                
                if (!isSuccessful)
                    callback?.Invoke(false);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                callback?.Invoke(false);
            }
        }

        private async void LobbyLimitReached()
        {
            var options = new UpdateLobbyOptions()
            {
                IsLocked = true
            };
            await Lobbies.Instance.UpdateLobbyAsync(_currentLobby.Id, options);
        }

        public async void DeleteCurrentLobby()
        {
            try
            {
                await Lobbies.Instance.DeleteLobbyAsync(_currentLobby.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}