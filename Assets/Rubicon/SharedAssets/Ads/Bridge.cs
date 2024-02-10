using UnityEngine;

namespace Rubicon.Ads
{
    public class Bridge : MonoBehaviour
    {
        public BridgeAdvertisement _ads { get; private set; }
        public BridgePlayer _player { get; private set; }
        public BridgeGame _game { get; private set; }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (gameObject.name != "Bridge") 
                gameObject.name = "Bridge";
        }
#endif

        private void Awake()
        {
            _ads = gameObject.AddComponent<BridgeAdvertisement>();
            _player = gameObject.AddComponent<BridgePlayer>();
            _game = gameObject.AddComponent<BridgeGame>();
        }
    }
}