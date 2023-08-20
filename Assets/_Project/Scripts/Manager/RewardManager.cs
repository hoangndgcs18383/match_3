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
            UIManager.Current.UpdatePowerUp(powerUpType);
        }

    }
}