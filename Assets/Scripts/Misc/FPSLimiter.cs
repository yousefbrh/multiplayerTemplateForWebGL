using UnityEngine;

namespace Misc
{
    public class FPSLimiter : Singleton<FPSLimiter>
    {
        public int waitMilliSeconds = 10;
        void Update()
        {
            if (Application.isEditor)
            {
                System.Threading.Thread.Sleep(waitMilliSeconds);
            }
        }
    }
}