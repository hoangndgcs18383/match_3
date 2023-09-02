using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Match_3
{
    public class UIMenu : ScreenBase
    {
        [SerializeField] private Button ppButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button backButton;

        protected override void OnEnable()
        {
            base.OnEnable();
            mainMenuButton.onClick.AddListener(OnMainMenuClick);
            backButton.onClick.AddListener(OnBackClick);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            mainMenuButton.onClick.RemoveListener(OnMainMenuClick);
            backButton.onClick.RemoveListener(OnBackClick);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnBackClick()
        {
            Hide();
        }

        private void OnMainMenuClick()
        {
            LoadingManager.Instance.LoadScene("Menu");
            Hide();
            UIManager.Current.HideGamePlayUI();
        }

    }
}