using UnityEngine;

namespace Managers
{
    public class BridgeDataManager : MonoBehaviour
    {
        public static BridgeDataManager Instance;
        
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
            }
        }

        public string GetPlatformId()
        {
            return "";
        }

        public string GetPlayerName()
        {
            return "";
        }

        public string GetAvatarId()
        {
            return "";
        }
    }
}