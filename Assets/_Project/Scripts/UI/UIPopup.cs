using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Match_3
{
    public class UIPopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text content;

        private Action _onBtnOke;
        private Action _onBtnCancel;

        private RectTransform _rectTransform;

        public RectTransform RectTransform
        {
            get
            {
                if(_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        public void Show(string mTitle, string mContent, Action onBtnOke, Action onBtnCancel)
        {
            DOTween.Kill(RectTransform.transform);
            RectTransform.localScale = Vector3.zero;
            TweenShow();
            title.text = mTitle;
            content.text = mContent;
            _onBtnOke = onBtnOke;
            _onBtnCancel = onBtnCancel;
            gameObject.SetActive(true);
        }

        public void OnBtnOke()
        {
            _onBtnOke?.Invoke();
            TweenHide();
        }

        public void OnBtnCancel()
        {
            _onBtnCancel?.Invoke();
            TweenHide();
        }

        private void TweenShow()
        {
            RectTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        }

        private void TweenHide()
        {
            RectTransform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack)
                .OnComplete(() => { gameObject.SetActive(false); });
        }

        private void OnDisable()
        {
            _onBtnOke = null;
            _onBtnCancel = null;
        }
    }
}