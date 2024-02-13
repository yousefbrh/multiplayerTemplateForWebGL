using UI;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Entities
{
    public class Player : NetworkBehaviour
    {
        private bool _isChatting;

        public NetworkVariable<int> ServerId = new NetworkVariable<int>();
        public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

        public void IsChatting(bool isActive)
        {
            _isChatting = isActive;
        }
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (IsOwner)
            {
                GameChat.Instance.PlayerInit(this);
            }
        }
        
        
    }
}