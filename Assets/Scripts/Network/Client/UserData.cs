using System;

namespace Network.Client
{
    [Serializable]
    public class UserData
    {
        public string userName;
        public string userAuthID;
        public int pointValue;
        public int teamIndex = -1;
        public int skinIndex;
    }
}