using System;
using System.Collections.Generic;
using MEC;
using TMPro;
using UnityEngine;
//using UnityEngine.Localization;
using UnityEngine.UI;

namespace Match_3
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text coinText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private UIPopup popup;
        [SerializeField] private Image spinAdsButton;

        private Dictionary<PowerUpType, PowerUpItem> powerUpItems = new Dictionary<PowerUpType, PowerUpItem>();

        public static UIManager Current;

        private void Awake()
        {
            Current = this;
        }

        #region PowerUp

        private void Start()
        {
            InitPowerUp();
        }

        private void InitPowerUp()
        {
            var powerUps = FindObjectsOfType<PowerUpItem>();
            
            foreach (var powerUp in powerUps)
            {
                powerUpItems.Add(powerUp.PowerUpType, powerUp);
                powerUpItems[powerUp.PowerUpType].Init(GetPowerUpCount(powerUp.PowerUpType), OnPowerUpClick);
            }
        }

        private void OnPowerUpClick(PowerUpType obj)
        {
            switch (obj)
            {
                case PowerUpType.Shuffle:
                    GameManager.Current.OnButtonShuffle();
                    break;
                case PowerUpType.Suggests:
                    GameManager.Current.OnButtonSuggest();
                    break;
                case PowerUpType.Undo:
                    GameManager.Current.OnButtonUndo();
                    break;
            }
        }

        private int GetPowerUpCount(PowerUpType powerUpPowerUpType)
        {
            return GameConfig.GetPowerUpCount(powerUpPowerUpType);
        }

        #endregion

        public void SetCoinText(int coin)
        {
            coinText.SetText(coin.ToString());
        }

        public void SetLevelText(int level)
        {
            levelText.SetText("Level " + level);
        }


        #region Ads

        public void ShowPopup(string title, string content, Action onBtnOke, Action onBtnCancel)
        {
            popup.Show(title, content, onBtnOke, onBtnCancel);
        }

        public void SpinAds()
        {
            Timing.RunCoroutine(IESpinAdsCoroutine());
        }

        private IEnumerator<float> IESpinAdsCoroutine()
        {
            yield return Timing.WaitForOneFrame;
            spinAdsButton.Spin();
        }

        public void StopSpinAds()
        {
            Timing.KillCoroutines();
            spinAdsButton.transform.localScale = Vector3.one;
        }

        #endregion
    }
}