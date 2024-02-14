using Misc;
using UnityEngine;

namespace UI
{
    public class LeavePanel : Panel
    {
        public void LeaveGame()
        {
            Utility.Leave();
        }
    }
}