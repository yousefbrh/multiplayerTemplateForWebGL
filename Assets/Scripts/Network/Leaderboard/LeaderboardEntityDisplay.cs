using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Network.Leaderboard
{
    public class LeaderboardEntityDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text pointsText;

        public void InitializeGlobal(string playerName, string value)
        {
            playerName = ShrinkPlayerName(playerName);
            nameText.text = playerName;
            pointsText.text = value;
        }

        private string ShrinkPlayerName(string playerName)
        {
            var idx = playerName.IndexOf('#');
            if (idx < 0) return null;
            var result = playerName.Substring(0, idx);
            return result;
        }
    }
}