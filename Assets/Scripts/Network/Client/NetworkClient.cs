using System;
using Managers;
using Misc;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace Network.Client
{
    public class NetworkClient : IDisposable
    {
        private NetworkManager _networkManager;


        public NetworkClient(NetworkManager networkManager)
        {
            _networkManager = networkManager;

            _networkManager.OnClientDisconnectCallback += OnClientDisconnect;
        }

        private void OnClientDisconnect(ulong clientId)
        {
            if (clientId != 0 && clientId != _networkManager.LocalClientId) return;
            
            Disconnect();
        }

        public void Dispose()
        {
            if (_networkManager != null)
            {
                _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
            }
        }

        public void Disconnect()
        {
            UIManager.Instance.ShowEntryPanel(true);
            UIManager.Instance.ShowChatPanel(false);
            GameManager.Instance.CancelMatchMade(true);
            GameManager.Instance.SetSceneActionSettings(false);

            if (SceneManager.GetActiveScene().name != Utility.EntrySceneName)
            {
                SceneManager.LoadScene(Utility.EntrySceneName);
            }

            if (_networkManager.IsConnectedClient)
            {
                _networkManager.Shutdown();
            }
        }
    }
}