using System;
using System.Text;
using System.Threading.Tasks;
using Components;
using Enums;
using Managers;
using Misc;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Network.Client
{
    public class ClientSingleton : MonoBehaviour
    {
        private JoinAllocation _joinAllocation;
        private NetworkClient _networkClient;
        private UserData _userData;
        public static ClientSingleton Instance;
        
        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
        }
        
        public async Task<bool> CreateClient()
        {
            return await InitAsync();
        }
        
        public async Task<bool> InitAsync()
        {
            await UnityServices.InitializeAsync();
            _networkClient = new NetworkClient(NetworkManager.Singleton);
            var authState = await AuthenticationWrapper.DoAuth();

            if (authState == AuthState.Authenticated)
            {
                return true;
            }

            return false;
        }

        private void UpdateUserData()
        {
            _userData = new UserData
            {
                userName = Prefs.GetPlayerName("Missing Name"),
                userAuthID = AuthenticationService.Instance.PlayerId,
                pointValue = PlayerPrefs.GetInt("Point"),
                skinIndex = PlayerPrefs.GetInt("CurrentSkinIndex", 0)
            };
        }

        public void StartClient(string ip, int port)
        {
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetConnectionData(ip, (ushort) port);
            ConnectClient();
        }

        public async Task StartClientAsync(string joinCode)
        {
            try
            {
                _joinAllocation = await Relay.Instance.JoinAllocationAsync(joinCode);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return;
            }
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            var relayServerData = new RelayServerData(_joinAllocation, "wss");
            transport.SetRelayServerData(relayServerData);
            
            ConnectClient();
        }

        private void ConnectClient()
        {
            UpdateUserData();
            var payload = JsonUtility.ToJson(_userData);
            var payloadBytes = Encoding.UTF8.GetBytes(payload);
            NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
            NetworkManager.Singleton.StartClient();
            GameManager.Instance.SetSceneActionSettings(true);
        }
        
        public void Dispose()
        {
            _networkClient?.Dispose();
        }

        public void Disconnect()
        {
            _networkClient.Disconnect();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}