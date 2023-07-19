using TMPro;
using UnityEngine;

namespace Match_3
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text levelText;

        public static UIManager Current;

        private void Awake()
        {
            Current = this;
        }

        public void SetLevelText(int level)
        {
            levelText.SetText("Level " + level);
        }
    }
}