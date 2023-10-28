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
        [SerializeField] private UIPopupRemoveAds uiPopupRemoveAds;
        [SerializeField] private GameObject uiGamePlay;

        [Title("Buttons")] [SerializeField] private Button menuButton;
        [SerializeField] private Button removeAdsButton;
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

            if (!ProfileDataService.Instance.IsRemoveAds())
            {
                IAPService.Instance.OnPurchaseCallbackIAPEvent += OnPurchaseCallbackIAPEvent;
            }
        }

        private void OnPurchaseCallbackIAPEvent(string productId, Status status)
        {
            Debug.Log($"[OnPurchaseCallbackIAPEvent] {productId} {status}");

            if (status == Status.Success)
            {
                FlyTextManager.Instance.SetFlyText("Remove Ads Success");
                ProfileDataService.Instance.SetRemoveAds(true);
                uiPopupRemoveAds.Hide();
                IAPService.Instance.OnPurchaseCallbackIAPEvent -= OnPurchaseCallbackIAPEvent;
            }
        }


        private void OnEnable()
        {
            menuButton.onClick.AddListener(OnMenuClick);
            removeAdsButton.onClick.AddListener(RemoveAds);
        }

        private void OnDisable()
        {
            menuButton.onClick.RemoveListener(OnMenuClick);
            removeAdsButton.onClick.RemoveListener(RemoveAds);
        }

        public void ShowPopupRemoveAds()
        {
            uiPopupRemoveAds.Show();
        }

        private void RemoveAds()
        {
            IAPService.Instance.Purchase(IAPPurchaseType.RemoveAds.ToString());
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
            var powerUps = FindObjectsOfType<PowerUpItem>(true);

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
            return;
            spinAdsButton.gameObject.SetActive(false);
        }

        #endregion
    }
}