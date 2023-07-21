using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Match_3
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private UIPopup popup;

        public static UIManager Current;
        
        private void Awake()
        {
            Current = this;
        }

        public void SetLevelText(int level)
        {
            levelText.SetText("Level " + level);
        }

        public void ShowPopup(string title, string content, Action onBtnOke, Action onBtnCancel)
        {
            popup.Show(title, content, onBtnOke, onBtnCancel);
        }
    }
}