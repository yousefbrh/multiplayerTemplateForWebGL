using System.Threading.Tasks;
using Misc;
using Network.Client;
using Network.Server;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Managers
{
    public class ApplicationManager : MonoBehaviour
    {
        [SerializeField] private ServerSingleton serverPrefab;
        [SerializeField] private ClientSingleton clientPrefab;
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private UnityTransport unityTransport;
        [SerializeField] private string url;

        public static ApplicationManager Instance;

        private bool _firstTimeCall = true;

        private void Awake()
        {
            if (NetworkManager.Singleton && Instance)
            {
                Destroy(networkManager.gameObject);
            }

            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public async void Start()
        {
            await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
        }

        private async Task LaunchInMode(bool isDedicatedServer)
        {
            if (isDedicatedServer)
            {
                Debug.Log("Lunch");
                var serverSingleton = Instantiate(serverPrefab);
                CreateServer(serverSingleton);
            }
            else
            {
                GameManager.Instance.IsAuthenticated(false);
                var authenticated = false;

                if (!ClientSingleton.Instance)
                {
                    var clientSingleton = Instantiate(clientPrefab);
                    authenticated = await clientSingleton.CreateClient();

                }
                else
                {
                    authenticated = await ClientSingleton.Instance.CreateClient();
                }

                if (authenticated)
                {
                    await Websocket.GetToken(url);
                    GameManager.Instance.IsAuthenticated(true);
                }
            }
        }

        private void CreateServer(ServerSingleton serverSingleton)
        {
            Debug.Log("Create Server");
#if !UNITY_WEBGL
            serverSingleton.CreateServer(unityTransport);
#endif
        }
    }
}