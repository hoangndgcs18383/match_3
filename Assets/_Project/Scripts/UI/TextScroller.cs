using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Match_3
{
    public class TextScroller : MonoBehaviour
    {
        public float loopDuration = 5f;
        private TMP_Text tmpText;
        private RectTransform rectTransform;
        private float textWidth;

        private void Start()
        {
            tmpText = GetComponent<TMP_Text>();
            rectTransform = GetComponent<RectTransform>();
            textWidth = tmpText.preferredWidth;
 
            StartLoop();
        }

        private void StartLoop()
        {
            rectTransform.DOAnchorPosX(textWidth / 2 + 150, loopDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() => ResetPosition());
        }

        private void ResetPosition()
        {
            rectTransform.anchoredPosition = new Vector2(-(textWidth / 2 +150), rectTransform.anchoredPosition.y);
            StartLoop();
        }
    }
}