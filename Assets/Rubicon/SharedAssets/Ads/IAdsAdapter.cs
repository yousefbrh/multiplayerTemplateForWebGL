using System;

namespace Ads
{
    public interface IAdsAdapter
    {
        public event Action RewardSuccess, RewardFailed;
        public event Action InterSuccess, InterFailed;
        
        public bool InterReady { get; }
        public bool RewardReady { get; }

        public void ShowReward();
        public void ShowInter();
    }
}