using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zeff.Core.Service;
using Zeff.Extensions;

namespace Match_3
{
    public class UIMainMenu : BaseScreen
    {
        [Title("Text")] [SerializeField] private TMP_Text coinText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text countDownText;
        [SerializeField] private TMP_Text livesText;

        [Title("Button")] [SerializeField] private Button playButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private Button questButton;
        [SerializeField] private Button removeAdsButton;

        [SerializeField] private GameObject shopPanel;
        [SerializeField] private RectTransform gameNameTransform;

        [Title("UI Quest")] [SerializeField] private UIQuest uiQuest;

        private bool _isShopPanelActive = false;


        protected override void OnEnable()
        {
            base.OnEnable();
            playButton.onClick.AddListener(OnPlayButtonClicked);
            shopButton.onClick.AddListener(OnShopButtonClicked);
            questButton.onClick.AddListener(OnButtonQuestClicked);

            removeAdsButton.gameObject.SetActive(!ProfileDataService.Instance.IsRemoveAds());
            removeAdsButton.onClick.AddListener(OnButtonRemoveAdsClicked);
            if (!ProfileDataService.Instance.IsRemoveAds())
            {
                IAPService.Instance.OnPurchaseCallbackIAPEvent += OnPurchaseCallbackIAPEvent;
            }

            gameNameTransform.DOLocalMoveY(gameNameTransform.localPosition.y + 10f, 1f).SetLoops(-1, LoopType.Yoyo);
            coinText.SetText($"{ProfileDataService.Instance.GetGold()}");
            livesText.SetText($"{ProfileDataService.Instance.ProfileData.Lives}");

            countDownText.SetText("");
            TimerManager.Current.CountDownCallback += OnCountDown;

            levelText.SetText("Level " + GameManager.Current.Level);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            playButton.onClick.RemoveListener(OnPlayButtonClicked);
            shopButton.onClick.RemoveListener(OnShopButtonClicked);
            questButton.onClick.RemoveListener(OnButtonQuestClicked);
            removeAdsButton.onClick.RemoveListener(OnButtonRemoveAdsClicked);
            gameNameTransform.DOKill();

            TimerManager.Current.CountDownCallback -= OnCountDown;
            if (!ProfileDataService.Instance.IsRemoveAds())
            {
                IAPService.Instance.OnPurchaseCallbackIAPEvent -= OnPurchaseCallbackIAPEvent;
            }
        }

        private void OnPurchaseCallbackIAPEvent(string productId, Status status)
        {
            if (status == Status.Success)
            {
                removeAdsButton.gameObject.SetActive(false);
                IAPService.Instance.OnPurchaseCallbackIAPEvent -= OnPurchaseCallbackIAPEvent;
            }
        }


        private void OnShopButtonClicked()
        {
            shopPanel.SetActive(true);
        }

        private void OnButtonRemoveAdsClicked()
        {
            UIManager.Current.ShowPopupRemoveAds();
        }

        public void OnPlayButtonClicked()
        {
            if (ProfileDataService.Instance.IsEnoughLife())
            {
                GameManager.Current.RestartLevel();
                gameObject.SetActive(false);
                UIManager.Current.ShowGamePlayUI();
            }
            else
            {
                FlyTextManager.Instance.SetFlyText("Not enough lives");
            }
        }

        private void OnButtonQuestClicked()
        {
            uiQuest.Show();
        }

        private void OnCountDown(float time)
        {
            countDownText.SetText(time.ToTimeFormat());
        }
    }
}