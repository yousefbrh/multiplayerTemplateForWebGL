using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Managers;
using Network.Client;
using Unity.Netcode;
using UnityEngine;

namespace Network.Server
{
    public class NetworkServer : IDisposable
    {
        private NetworkManager _networkManager;
        private Dictionary<ulong, string> _clientIdToAuth = new Dictionary<ulong, string>();
        private Dictionary<string, UserData> _authIdToUserData = new Dictionary<string, UserData>();

        public Action<string> onClientLeft;
        
        public NetworkServer(NetworkManager networkManager)
        {
            _networkManager = networkManager;
            _networkManager.ConnectionApprovalCallback += ApprovalCheck;
            _networkManager.OnServerStarted += OnNetworkReady;
        }
        
        private void ApprovalCheck(
            NetworkManager.ConnectionApprovalRequest request, 
            NetworkManager.ConnectionApprovalResponse response)
        {
            var payload = System.Text.Encoding.UTF8.GetString(request.Payload);
            var userData = JsonUtility.FromJson<UserData>(payload);
            
            Debug.Log("Approval Check");
            _clientIdToAuth[request.ClientNetworkId] = userData.userAuthID;
            _authIdToUserData[userData.userAuthID] = userData;
            
            response.Approved = true;
            response.CreatePlayerObject = false;
        }
        
        private void OnNetworkReady()
        {
            _networkManager.OnClientConnectedCallback += OnClientConnected;
            _networkManager.OnClientDisconnectCallback += OnClientDisconnect;
            _ = CheckForDestroy();
        }
        
        private async Task CheckForDestroy()
        {
            Debug.Log("CheckForDestroy1");
            await Task.Delay(10000);
            Debug.Log("CheckForDestroy2");
            if (_clientIdToAuth.Count == 0)
            {
                LobbyManager.Instance.DeleteCurrentLobby();
                Dispose();
            }
        }
        
        private void OnClientConnected(ulong clientId)
        {
            Debug.Log("Client count : " +  _clientIdToAuth.Count);
        }

        private void OnClientDisconnect(ulong clientId)
        {
            Debug.Log("client disconnected");
            if (_clientIdToAuth.TryGetValue(clientId, out var authId))
            {
                _clientIdToAuth.Remove(clientId);
                _authIdToUserData.Remove(authId);
                onClientLeft?.Invoke(authId);
            }
            Debug.Log("Client count : " +  _clientIdToAuth.Count);
            if (_clientIdToAuth.Count == 0)
            {
                LobbyManager.Instance.DeleteCurrentLobby();
                Dispose();
            }
        }
        
        public UserData GetUserDataByClientId(ulong clientId)
        {
            if (!_clientIdToAuth.TryGetValue(clientId, out var authId)) return null;
            return _authIdToUserData.TryGetValue(authId, out var data) ? data : null;
        }

        public void Dispose()
        {
            if (_networkManager == null) return;
            _networkManager.ConnectionApprovalCallback -= ApprovalCheck;
            _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
            _networkManager.OnClientConnectedCallback -= OnClientConnected;
            _networkManager.OnServerStarted -= OnNetworkReady;

            if (_networkManager.IsListening)
            {
                Debug.Log("Shutdown Called");
                _networkManager.Shutdown();
                Application.Quit();
            }
        }
    }
}