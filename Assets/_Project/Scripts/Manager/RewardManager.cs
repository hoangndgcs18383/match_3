using UnityEngine;

namespace Match_3
{
    public class RewardManager : MonoBehaviour
    {
        public static RewardManager Current;

        private void Awake()
        {
            if (Current == null)
            {
                Current = this;
                if (transform.parent == null)
                {
                    DontDestroyOnLoad(this);
                }
            }
        }
        
        public void AddRandomPowerUp()
        {
            var powerUpType = (PowerUpType)Random.Range(0, 3);
            AddPowerUp(powerUpType, 1);
        }

        private void AddPowerUp(PowerUpType powerUpType, int count)
        {
            GameConfig.AddPowerUp(powerUpType, count);
            UIManager.Current.UpdateGUIPowerUp(powerUpType);
        }

        public void AddReward(PowerUpType powerUpType, int i)
        {
            AddPowerUp(powerUpType, i);
        }
        
        public void UpdateCoinView()
        {
            UIManager.Current.SetCoinText(GetCoin());
        }

        public int GetRandomCoin(bool isLose = false)
        {
            return !isLose ? Random.Range(1000, 2000) : Random.Range(100, 500);
        }

        public void AddCoin(int coin)
        {
            ProfileDataService.Instance.AddGold(coin);
            UpdateCoinView();
        }

        private int GetCoin()
        {
            return ProfileDataService.Instance.GetGold();
        }
    }
}