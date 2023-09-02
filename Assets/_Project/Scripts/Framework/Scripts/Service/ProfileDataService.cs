using System;
using System.Collections.Generic;
using UnityEngine;
using Zeff.Core.Parser;
using Zeff.Core.SaveGame;
using Zeff.Core.Service;
using Zeff.Extensions;

namespace Match_3
{
    public class ProfileDataService : Service<ProfileDataService>
    {
        #region Default Profile Data

        private readonly ProfileData _defaultProfileData = new ProfileData
        {
            Level = 1,
            Lives = 5,
            Gold = 0,
            LastTimePlay = DateTime.Now.Ticks,
            LastTimeReceiveLife = new LastTimeReceiveLife
            {
                TotalSecond = 0
            },
            PowerUpData = new PowerUpData
            {
                Undo = 3,
                Shuffle = 3,
                Suggests = 3
            }
        };

        #endregion

        public ProfileData ProfileData { get; private set; }
        public float TimeAutoSave = 60f;
        public int LastTimeReceiveLife = 900; // 15 minutes = 900 seconds

        private const string ProfileDataKey = "ProfileData";

        public override void Initialize()
        {
            base.Initialize();


            if (ZSaveGame.HasKey(ProfileDataKey))
            {
                ZSaveGame.LoadLocal<ProfileData>(ProfileDataKey, (profileData) =>
                {
                    ProfileData = profileData;
                    DebugFormat.ToString(ProfileData);
                });
            }
            else
            {
#if UNITY_EDITOR
                ZSaveGame.CreateDefault(_defaultProfileData, ProfileDataKey);
#else
                ZSaveGame.CreateDefault(_defaultProfileData, ProfileDataKey, DataPath.PersistentDataPath);
#endif
                ProfileData = _defaultProfileData;
            }

            if (ProfileData.LastTimeReceiveLife.TotalSecond > 0)
            {
                ProfileData.LastTimeReceiveLife.TotalSecond -= CalculateOfflineTime();
            }
            else
            {
                ProfileData.LastTimeReceiveLife.TotalSecond = 0;
            }
        }

        private int CalculateOfflineTime()
        {
            //Calculate offline time
            int result = 0;
            long currentTime = DateTime.Now.Ticks;
            currentTime = currentTime - ProfileData.LastTimePlay;
            TimeSpan timeSpan = new TimeSpan(currentTime);

            if (ProfileData.Lives < 5)
                ProfileData.Lives += (int)(timeSpan.TotalSeconds / 900); // 15 minutes = 900 seconds

            if (ProfileData.Lives > 5)
                ProfileData.Lives = 5;

            DebugFormat.ToString((int)timeSpan.TotalSeconds);
            result = (int)(timeSpan.TotalSeconds);
            return result;
        }

        public void SaveProfileData()
        {
            ProfileData.LastTimePlay = DateTime.Now.Ticks;

#if UNITY_EDITOR
            ZSaveGame.SaveLocal(ProfileData, ProfileDataKey);
#else
            ZSaveGame.SaveLocal(ProfileData, ProfileDataKey, DataPath.PersistentDataPath);
#endif
            Debug.Log($"[ProfileData] Save Profile {ProfileData.ToJson()}");
        }

        public void SetProfileData(ProfileData profileData)
        {
            ProfileData = profileData;
            SaveProfileData();
        }

        public void KillLife(Action onNextCallback, Action onBackCallback)
        {
            if (ProfileData.Lives > 0)
            {
                ProfileData.Lives--;
                SaveProfileData();
                onNextCallback?.Invoke();
            }
            else
            {
                onBackCallback?.Invoke();
                Debug.Log("Not enough lives");
            }
        }

        public void AddGold(int value)
        {
            ProfileData.Gold += value;
            SaveProfileData();
        }

        public int GetGold()
        {
            return ProfileData.Gold;
        }

        public void SetLastTimeReceiveLife(int value)
        {
            ProfileData.LastTimeReceiveLife.TotalSecond = value;
        }

        public void AddLastTimeReceiveLife()
        {
            ProfileData.LastTimeReceiveLife.TotalSecond += LastTimeReceiveLife;
        }

        public float GetLastTimeReceiveLife()
        {
            return ProfileData.LastTimeReceiveLife.TotalSecond;
        }

        public void SetLevel(int value)
        {
            ProfileData.Level = value;
            SaveProfileData();
        }

        public void UpdateNextLevel()
        {
            ProfileData.Level++;
            SaveProfileData();
        }

        #region Handle PowerUp

        public int GetPowerUpData(PowerUpType powerUpType)
        {
            switch (powerUpType)
            {
                case PowerUpType.Shuffle:
                    return ProfileData.PowerUpData.Shuffle;
                case PowerUpType.Suggests:
                    return ProfileData.PowerUpData.Suggests;
                case PowerUpType.Undo:
                    return ProfileData.PowerUpData.Undo;
            }

            return 0;
        }

        public void AddPowerUpData(PowerUpType powerUpType, int count)
        {
            switch (powerUpType)
            {
                case PowerUpType.Shuffle:
                    ProfileData.PowerUpData.Shuffle += count;
                    break;
                case PowerUpType.Suggests:
                    ProfileData.PowerUpData.Suggests += count;
                    break;
                case PowerUpType.Undo:
                    ProfileData.PowerUpData.Undo += count;
                    break;
            }
        }

        public void UsePowerUpData(PowerUpType powerUpType, Action onUsePowerUp = null, Action<PowerUpType> onCancelPowerUp = null)
        {
            switch (powerUpType)
            {
                case PowerUpType.Shuffle:
                    OnShuffleUse(powerUpType, onUsePowerUp, onCancelPowerUp);
                    break;
                case PowerUpType.Suggests:
                    OnSuggestUse(powerUpType, onUsePowerUp, onCancelPowerUp);
                    break;
                case PowerUpType.Undo:
                    OnUndoUse(powerUpType, onUsePowerUp, onCancelPowerUp);
                    break;
            }
        }

        private void OnUndoUse(PowerUpType powerUpType, Action onUsePowerUp, Action<PowerUpType> onCancelPowerUp)
        {
            if (ProfileData.PowerUpData.Undo > 0)
            {
                ProfileData.PowerUpData.Undo--;
                onUsePowerUp?.Invoke();
            }
            else
            {
                onCancelPowerUp?.Invoke(powerUpType);
            }
        }

        private void OnSuggestUse(PowerUpType powerUpType, Action onUsePowerUp, Action<PowerUpType> onCancelPowerUp)
        {
            if (ProfileData.PowerUpData.Suggests > 0)
            {
                ProfileData.PowerUpData.Suggests--;
                onUsePowerUp?.Invoke();
            }
            else
            {
                onCancelPowerUp?.Invoke(powerUpType);
            }
        }

        private void OnShuffleUse(PowerUpType powerUpType, Action onUsePowerUp, Action<PowerUpType> onCancelPowerUp)
        {
            if (ProfileData.PowerUpData.Shuffle > 0)
            {
                ProfileData.PowerUpData.Shuffle--;
                onUsePowerUp?.Invoke();
            }
            else
            {
                onCancelPowerUp?.Invoke(powerUpType);
            }
        }

        public void BuyPowerUpData(Dictionary<string, int> rewards, int price, Action onBuySuccess = null,
            Action onBuyFail = null)
        {
            if (ProfileData.Gold < price)
            {
                Debug.Log("Not enough gold");
                onBuyFail?.Invoke();
                return;
            }

            Debug.Log($"[ProfileData] Buy PowerUp {rewards.ToJson()}");

            
            if (rewards.TryGetValue("ITEM_SHUFFLE", out var rewardShuffle))
            {
                ProfileData.PowerUpData.Shuffle += rewardShuffle;
                Debug.Log($"[ProfileData] Buy PowerUp Shuffle {rewardShuffle}");
            }

            if (rewards.TryGetValue("ITEM_UNDO", out var rewardUndo))
            {
                ProfileData.PowerUpData.Undo += rewardUndo;
                Debug.Log($"[ProfileData] Buy PowerUp Undo {rewardUndo}");
            }

            if (rewards.TryGetValue("ITEM_SUGGEST", out var rewardSuggests))
            {
                ProfileData.PowerUpData.Suggests += rewardSuggests;
                Debug.Log($"[ProfileData] Buy PowerUp Suggests {rewardSuggests}");
            }

            ProfileData.Gold -= price;
            SaveProfileData();
            onBuySuccess?.Invoke();
        }
        

        #endregion
        

        //Example
        public void AutoSaveProfile()
        {
            TimingService.Instance.AddTimer(TimeAutoSave, SaveProfileData, -1);
        }
    }
}