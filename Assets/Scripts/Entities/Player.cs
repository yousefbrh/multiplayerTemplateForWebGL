using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Entities
{
    public class Player : NetworkBehaviour
    {
        public NetworkVariable<int> ServerId = new NetworkVariable<int>();
        public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
        }
        
        
    }
}