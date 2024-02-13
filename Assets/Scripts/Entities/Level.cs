using System;
using Unity.Netcode;
using UnityEngine;

namespace Entities
{
    public class Level : MonoBehaviour
    {
        [SerializeField] private NetworkObject playerPrefab;

        private void Start()
        {
            if (!NetworkManager.Singleton.IsServer) return;
            NetworkManager.Singleton.OnClientConnectedCallback += Client;
        }

        private void Client(ulong obj)
        {
            var clone = Instantiate(playerPrefab, transform);
            clone.SpawnAsPlayerObject(obj);
        }
        
        private void OnDestroy()
        {
            var networkManager = NetworkManager.Singleton;
            if (networkManager)
                NetworkManager.Singleton.OnClientConnectedCallback -= Client;
        }
    }
}