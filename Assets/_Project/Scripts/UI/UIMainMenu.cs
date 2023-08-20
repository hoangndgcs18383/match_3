using TMPro;
using UnityEngine;

namespace Match_3
{
    public class UIMainMenu : ScreenBase
    {
        [SerializeField] private TMP_Text levelText;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            levelText.SetText("Level " + GameManager.Current.Level);
        }
    }
}