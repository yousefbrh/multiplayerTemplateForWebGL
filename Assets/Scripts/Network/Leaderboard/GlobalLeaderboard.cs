using System.Collections.Generic;
using System.Globalization;
using Unity.Services.Leaderboards;
using UnityEngine;

namespace Network.Leaderboard
{
    public class GlobalLeaderboard : MonoBehaviour
    {
        #region SerializeField
        [SerializeField] private Transform leaderboardEntityHolder;
        [SerializeField] private LeaderboardEntityDisplay leaderboardEntityPrefab;
        [SerializeField] private string leaderboardId;
        [SerializeField] private int maxShowEntities;
        #endregion

        #region Private
        #endregion

        #region Public
        public List<LeaderboardEntityDisplay> leaderboardEntityDisplays = new List<LeaderboardEntityDisplay>();
        public static GlobalLeaderboard Instance;
        #endregion
        
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
                return;
            }
        }
        
        public async void UpdateLeaderboard()
        {
            DeleteList();
            await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, 0);
            var leaderboardScoresPage = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId);
            if (!Application.isPlaying) return;
            var loopLimit = leaderboardScoresPage.Results.Count >= maxShowEntities
                ? maxShowEntities
                : leaderboardScoresPage.Results.Count;
            for (var i = 0; i < loopLimit; i++)
            {
                var entity = Instantiate(leaderboardEntityPrefab, leaderboardEntityHolder);
                entity.InitializeGlobal(leaderboardScoresPage.Results[i].PlayerName, leaderboardScoresPage.Results[i].Score.ToString(CultureInfo.InvariantCulture));
                leaderboardEntityDisplays.Add(entity);
            }
        }

        public async void ChangeScore(int value)
        {
            await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, value);
            UpdateLeaderboard();
        }

        private void DeleteList()
        {
            foreach (var entityDisplay in leaderboardEntityDisplays)
            {
                Destroy(entityDisplay.gameObject);
            }
            leaderboardEntityDisplays.Clear();
        }

        private void OnDestroy()
        {
            DeleteList();
        }
    }
}