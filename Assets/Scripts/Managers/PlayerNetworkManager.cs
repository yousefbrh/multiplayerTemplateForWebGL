using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Unity.Netcode;
using UnityEngine;

namespace Managers
{
    public class PlayerNetworkManager : NetworkBehaviour
    {
        [SerializeField] private List<PlayerSpotModel> playerSpotModels = new List<PlayerSpotModel>();
        [SerializeField] private int playerFullLimit = 4;
        [SerializeField] private int playerMinLimit = 2;

        private int _playerAdded;
        private bool _lobbyClosed;

        public static PlayerNetworkManager Instance;

        public Action<PlayerSpotModel> onPlayerStateChanged;
        public Action onPlayersLimitReached;
        
        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void PlayerAdded(Player player)
        {
            var playerSpotModel = playerSpotModels.First(model => !model.IsEquipped);
            if (IsServer)
                player.ServerId.Value = playerSpotModel.PlayerId;
            playerSpotModel.IsEquipped = true;
            playerSpotModel.Player = player;
            SyncSpots();
            if (!IsServer) return;
            if (playerSpotModels.FindAll(model => model.IsEquipped).Count == playerFullLimit)
            {
                onPlayersLimitReached?.Invoke();
                _lobbyClosed = true;
            }
        }
        
        public void PlayerRemoved(Player player)
        {
            var playerSpotModel = playerSpotModels.Find(model => model.PlayerId == player.ServerId.Value);
            playerSpotModel.IsEquipped = false;
            playerSpotModel.Player = null;
        }
        
        public void SyncSpots()
        {
            foreach (var player in playerSpotModels.Where(player => player.IsEquipped))
            {
                onPlayerStateChanged?.Invoke(player);
            }
        }

        public bool IsPlayerOwner(string playerName)
        {
            var player = playerSpotModels.Find(model => model.Player.PlayerName.Value == playerName);
            return player.Player.IsOwner;
        }
    }
    
    [Serializable]
    public class PlayerSpotModel
    {
        public int PlayerId;
        public bool IsEquipped;
        public Player Player;
    }
}