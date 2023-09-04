using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zeff.Extensions;

namespace Match_3
{
    public class UIMainMenu : BaseScreen
    {
        [SerializeField] private TMP_Text levelText;
        
        [Title("Button")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private Button questButton;
        
        [SerializeField] private GameObject shopPanel;
        
        [SerializeField] private GameObject[] livesGameObjects;
        [SerializeField] private TMP_Text countDownText;
        
        [Title("UI Quest")]
        [SerializeField] private UIQuest uiQuest;
        
        private bool _isShopPanelActive = false;
        

        protected override void OnEnable()
        {
            base.OnEnable();
            playButton.onClick.AddListener(OnPlayButtonClicked);
            shopButton.onClick.AddListener(OnShopButtonClicked);
            questButton.onClick.AddListener(OnButtonQuestClicked);
            
            
            countDownText.SetText("");
            UpdateLives();
            TimerManager.Current.CountDownCallback += OnCountDown;

            levelText.SetText("Level " + GameManager.Current.Level);
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            playButton.onClick.RemoveListener(OnPlayButtonClicked);
            shopButton.onClick.RemoveListener(OnShopButtonClicked);
            questButton.onClick.RemoveListener(OnButtonQuestClicked);
            
            TimerManager.Current.CountDownCallback -= OnCountDown;
        }

        private void OnShopButtonClicked()
        {
            shopPanel.SetActive(true);
        }

        
        public void OnPlayButtonClicked()
        {
            GameManager.Current.RestartLevel();
            UIManager.Current.ShowGamePlayUI();
        }
        private void OnButtonQuestClicked()
        {
            uiQuest.Show();
        }
        
        private void UpdateLives()
        {
            for (int i = 0; i < livesGameObjects.Length; i++)
            {
                livesGameObjects[i].SetActive(i < ProfileDataService.Instance.ProfileData.Lives);
            }
            
        }
        
        private void OnCountDown(float time)
        {
            countDownText.SetText(time.ToTimeFormat());
        }
    }
}