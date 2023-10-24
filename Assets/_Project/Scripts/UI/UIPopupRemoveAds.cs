
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Match_3
{
    public class UIPopupRemoveAds : MonoBehaviour
    {
        [SerializeField] private TMP_Text skipText;
        
        private void Start()
        {
            skipText.DOFade(1, 0.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
