using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zeff.Core.Localization;

namespace Match_3
{
    public class UIPopup : MonoBehaviour
    {
        [SerializeField] private LocalizationTMPText statusTxt;
        [SerializeField] private TMP_Text goldText;
        [SerializeField] private TMP_Text goldX3Text;
        [SerializeField] private TMP_Text noThanksText;
        [SerializeField] private Button btnCollect;
        [SerializeField] private Button btnNext;
        [SerializeField] private RectTransform targetTransform;

        private Action _onBtnCollect;
        private Action _onBtnNext;
        
        private bool _isAvailableClick = true;


        public RectTransform TargetTransform
        {
            get
            {
                if(targetTransform == null)
                    targetTransform = GetComponent<RectTransform>();
                return targetTransform;
            }
        }

        [Button]
        public void Show(string mTitle, int mGoldText, Action onBtnCollect, Action onBtnNext)
        {
            DOTween.Kill(TargetTransform.transform);
            TargetTransform.localScale = Vector3.zero;
            gameObject.SetActive(true);
            btnCollect.interactable = false;
            btnNext.interactable = false;
            noThanksText.gameObject.SetActive(false);
            
            AnimCollectButton();
            AnimTextCount(mGoldText);
            TweenShow();
            
            //statusTxt.SetText(mTitle);
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
            TargetTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        }
        
        private void AnimTextCount(int count)
        {
            goldText.DOText(count.ToString(), 0.5f, true, ScrambleMode.Numerals, "0123456789").SetEase(Ease.Linear).OnComplete(() =>
            {
                noThanksText.gameObject.SetActive(true);
                noThanksText.color = new Color(1, 1, 1, 0);
                noThanksText.DOFade(1, 0.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
                btnCollect.interactable = true;
                btnNext.interactable = true;
            });
            goldX3Text.DOText((count * 3).ToString(), 0.5f, true, ScrambleMode.Numerals, "0123456789").SetEase(Ease.Linear);
        }
        
        private void AnimCollectButton()
        {
            btnCollect.transform.DrillScale();
        }

        private void TweenHide()
        {
            TargetTransform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack)
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