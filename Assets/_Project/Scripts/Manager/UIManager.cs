using System;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
//using UnityEngine.Localization;
using UnityEngine.UI;
using Zeff.Core.Localization;
using Zeff.Core.Service;

namespace Match_3
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text coinText;
        [SerializeField] private LocalizationTMPText levelText;

        [Title("References")] [SerializeField] private UIPopup popup;
        [SerializeField] private UIMenu menu;
        [SerializeField] private UIReward uiReward;
        [SerializeField] private GameObject uiGamePlay;

        [Title("Buttons")] [SerializeField] private Button menuButton;
        [SerializeField] private Image spinAdsButton;

        private Dictionary<PowerUpType, PowerUpItem> powerUpItems = new Dictionary<PowerUpType, PowerUpItem>();

        public static UIManager Current;

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

        #region PowerUp

        private void Start()
        {
            InitPowerUp();
            
            IAPService.Instance.OnPurchaseCallbackIAPEvent += OnPurchaseCallbackIAPEvent;
        }

        private void OnPurchaseCallbackIAPEvent(Status status)
        {
            Debug.Log($"[OnPurchaseCallbackIAPEvent] {status}");
        }

        private void OnEnable()
        {
            menuButton.onClick.AddListener(OnMenuClick);
        }

        private void OnDisable()
        {
            menuButton.onClick.RemoveListener(OnMenuClick);
        }

        private void OnMenuClick()
        {
            menu.Show();
        }

        public void ShowGamePlayUI()
        {
            uiGamePlay.SetActive(true);
        }

        public void HideGamePlayUI()
        {
            uiGamePlay.SetActive(false);
        }

        [Button]
        public void ShowRewardUI(Dictionary<string, int> rewards)
        {
            uiReward.StartReward(rewards, RewardManager.Current.UpdateCoinView);
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
            levelText.SetParams(level);
        }

        public void UpdateGUIPowerUp(PowerUpType powerUpType)
        {
            powerUpItems[powerUpType].UpdateCount(GetPowerUpCount(powerUpType));
        }

        public void UpdateGUIAllPowerUp()
        {
            foreach (var powerUpItem in powerUpItems)
            {
                powerUpItem.Value.UpdateCount(GetPowerUpCount(powerUpItem.Key));
            }
        }


        #region Ads

        public void ShowPopup(string title, int content, Action onBtnCollect, Action onBtnNext)
        {
            popup.Show(title, content, onBtnCollect, onBtnNext);
        }

        public void SpinAds()
        {
            return;
            Timing.RunCoroutine(IESpinAdsCoroutine());
        }

        private IEnumerator<float> IESpinAdsCoroutine()
        {
            spinAdsButton.gameObject.SetActive(true);
            yield return Timing.WaitForOneFrame;
            spinAdsButton.transform.Spin();
        }

        public void StopSpinAds()
        {
            Timing.KillCoroutines();
        }

        public void DisableAds()
        {
            IAPService.Instance.Purchase(IAPPurchaseType.RemoveAds);
            return;
            spinAdsButton.gameObject.SetActive(false);
        }

        #endregion
    }
}