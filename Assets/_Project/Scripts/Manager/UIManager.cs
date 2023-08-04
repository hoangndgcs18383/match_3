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
        //[SerializeField] private LocalizedString levelString;

        public static UIManager Current;
        
        private void Awake()
        {
            Current = this;
        }
        
        public void SetCoinText(int coin)
        {
            coinText.SetText(coin.ToString());
        }

        public void SetLevelText(int level)
        {
            levelText.SetText("Level " + level);
        }

        #region PowerUp
        
        public void OnSuggestClick()
        {
            GameManager.Current.OnButtonSuggest();
        }
        
        public void OnShuffleClick()
        {
            GameManager.Current.OnButtonShuffle();
        }
        
        public void OnUndoClick()
        {
            GameManager.Current.OnButtonUndo();
        }

        #endregion


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