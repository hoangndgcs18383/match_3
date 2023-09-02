using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match_3
{
    public class UIPopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text statusTxt;
        [SerializeField] private TMP_Text goldText;
        [SerializeField] private TMP_Text goldX3Text;
        [SerializeField] private Button btnCollect;
        [SerializeField] private Button btnNext;

        private Action _onBtnCollect;
        private Action _onBtnNext;

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

        public void Show(string mTitle, int mGoldText, Action onBtnCollect, Action onBtnNext)
        {
            DOTween.Kill(RectTransform.transform);
            RectTransform.localScale = Vector3.zero;
            gameObject.SetActive(true);
            
            AnimTextCount(mGoldText);
            TweenShow();
            
            statusTxt.SetText(mTitle);
            _onBtnCollect = onBtnCollect;
            _onBtnNext = onBtnNext;
        }

        private void OnEnable()
        {
            btnCollect.onClick.AddListener(OnBtnCollect);
            btnNext.onClick.AddListener(OnBtnNext);
        }
        

        public void OnBtnCollect()
        {
            _onBtnCollect?.Invoke();
            TweenHide();
        }

        public void OnBtnNext()
        {
            _onBtnNext?.Invoke();
            TweenHide();
        }

        private void TweenShow()
        {
            RectTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        }
        
        private void AnimTextCount(int count)
        {
            goldText.DOText(count.ToString(), 0.5f, true, ScrambleMode.Numerals, "0123456789").SetEase(Ease.Linear);
            goldX3Text.DOText((count * 3).ToString(), 0.5f, true, ScrambleMode.Numerals, "0123456789").SetEase(Ease.Linear);
        }

        private void TweenHide()
        {
            RectTransform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack)
                .OnComplete(() => { gameObject.SetActive(false); });
        }

        private void OnDisable()
        {
            btnCollect.onClick.RemoveListener(OnBtnCollect);
            btnNext.onClick.RemoveListener(OnBtnNext);
            
            _onBtnCollect = null;
            _onBtnNext = null;
        }
    }
}