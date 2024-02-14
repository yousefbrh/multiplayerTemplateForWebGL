using Unity.Netcode;
using UnityEngine;

namespace Managers
{
    public class LevelManager : NetworkBehaviour
    {
        public NetworkVariable<int> levelNumber;
        private int _currentPrefabSuffix;

        public static LevelManager Instance;

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

        public override void OnNetworkSpawn()
        {
            if (IsServer)
                ServerDesign();
            else
                ClientDesign();
        }

        public void PrepareLevel()
        {
            if (IsServer)
                ServerDesign();
        }
        
        private void ServerDesign()
        {
            // _currentPrefabSuffix = Random.Range(1, 6);
            _currentPrefabSuffix = 1;
            var resourceRequest = Resources.LoadAsync($"Prefabs/Levels/Level {_currentPrefabSuffix}", typeof(GameObject));
            resourceRequest.completed += operation =>
            {
                Instantiate(resourceRequest.asset as GameObject);
                levelNumber.Value = _currentPrefabSuffix;
            };
        }
    
        private void ClientDesign()
        {
            var resourceRequest = Resources.LoadAsync($"Prefabs/Levels/Level {levelNumber.Value}", typeof(GameObject));
            resourceRequest.completed += operation =>
            {
                Instantiate(resourceRequest.asset as GameObject);
            };
        }
    }
}