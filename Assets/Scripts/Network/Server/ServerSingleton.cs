using System;
using Cysharp.Threading.Tasks;
using Managers;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using UnityEngine;
using Misc;
#if !UNITY_WEBGL
using Unity.Services.Multiplay;
#endif

namespace Network.Server
{
    public class ServerSingleton : MonoBehaviour
    {
        private string _allocationId;
        private NetworkServer _networkServer;
#if !UNITY_WEBGL
        private IServerQueryHandler _serverQueryHandler;
#endif
        public static ServerSingleton Instance;

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
            
#if !UNITY_WEBGL
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;
#else
            Application.runInBackground = true;
#endif
        }
        
#if !UNITY_WEBGL
        private void Update()
        {
            _serverQueryHandler?.UpdateServerCheck();
        }

        public void CreateServer(UnityTransport unityTransport)
        {
            _networkServer = new NetworkServer(NetworkManager.Singleton);
            StartServer(unityTransport).Forget();
        }

        private async UniTaskVoid StartServer(UnityTransport unityTransport)
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                await UnityServices.InitializeAsync(new InitializationOptions()).AsUniTask();
               
                string name = "server" + Guid.NewGuid().ToString().Substring(0, 10); 
                AuthenticationService.Instance.SwitchProfile(name);
                await AuthenticationService.Instance.SignInAnonymouslyAsync().AsUniTask();
                Debug.Log("Authenticated");
                try
                {
                    var m_MultiplayEventCallbacks = new MultiplayEventCallbacks(); 
                    m_MultiplayEventCallbacks.Allocate += OnAllocate; 
                    m_MultiplayEventCallbacks.Deallocate += OnDeallocate; 
                    m_MultiplayEventCallbacks.Error += OnError; 
                    m_MultiplayEventCallbacks.SubscriptionStateChanged += OnSubscriptionStateChanged;
                    Debug.Log("set up actions");
                    await MultiplayService.Instance.SubscribeToServerEventsAsync(m_MultiplayEventCallbacks).AsUniTask();
                    Debug.Log("subscribe multiplay");
                    _serverQueryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync(6, "SNR1", "", "1.0", "1").AsUniTask();
                    await AwaitAllocation();

                    var allocationRelay = await RelayService.Instance.CreateAllocationAsync(4);
                    Debug.Log("Get aloc");
                    var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocationRelay.AllocationId);
                    Debug.Log("get join code : " + joinCode);
                    RelayServerData relayServerData = new RelayServerData(allocationRelay, "wss");
                    unityTransport.SetRelayServerData(relayServerData);
                    await LobbyManager.Instance.CreateLobby(joinCode, _allocationId);
                    Debug.Log("network server");
                    NetworkManager.Singleton.StartServer();
                    StartCoroutine(Utility.LoadScene("Main", 0.5f));
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }
            }
        }
        
        
        private void OnAllocate(MultiplayAllocation obj)
        {
            _allocationId = obj.AllocationId;
        }
        
        private void OnDeallocate(MultiplayDeallocation obj)
        {
            print("Deallocate - " + obj.ServerId + " - " + obj.AllocationId + " - " + obj.EventId);
        }
        
        private void OnError(MultiplayError obj)
        {
            print("Error - " + obj.Reason + " - " + obj.Detail);
        }
        
        private void OnSubscriptionStateChanged(MultiplayServerSubscriptionState obj)
        {
            print("SubscriptionStateChanged : " + obj);
        }
#endif    
        
        
        private async UniTask AwaitAllocation()
        {
            while (_allocationId == "")
            {
                await UniTask.Yield();
            }
        }

        private void OnDestroy()
        {
            _networkServer?.Dispose();
        }
    }
}