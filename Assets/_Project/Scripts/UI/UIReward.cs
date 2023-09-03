using System;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match_3
{
    public class UIReward : BaseScreen
    {
        [SerializeField] private GameObject rewardPanel;
        [SerializeField] private GameObject lightObj;
        [SerializeField] private Image iconReward;
        [SerializeField] private Button bgButton;
        [SerializeField] private TMP_Text continueText;

        private Sequence _rewardSequence;
        private bool _isClickAvaiable = true;

        protected override void OnEnable()
        {
            base.OnEnable();
            rewardPanel.SetActive(false);
            _isClickAvaiable = false;
            bgButton.onClick.AddListener(OnBgClick);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            bgButton.onClick.RemoveListener(OnBgClick);
        }

        private void OnBgClick()
        {
            if (_isClickAvaiable)
            {
                StopRewardView();
            }
        }

        public void StartReward(Dictionary<string, int> rewards, Action callback = null)
        {
            //iconReward.sprite = IconManager.Current.GetIcon(id);
            Timing.RunCoroutine(IEAnimReward(rewards, callback));
        }

        private IEnumerator<float> IEAnimReward(Dictionary<string, int> rewards, Action callback = null)
        {
            continueText.color = new Color(1f, 1f, 1f, 0f);
            SetRotateLight();
            gameObject.SetActive(true);

            foreach (var reward in rewards)
            {
                iconReward.sprite = IconManager.Current.GetIcon(reward.Key);
                rewardPanel.SetActive(true);
                rewardPanel.transform.localScale = Vector3.zero;
                _rewardSequence = DOTween.Sequence();
                _rewardSequence.Append(rewardPanel.transform.DOScale(1, 0.1f).SetEase(Ease.OutQuad));

                yield return Timing.WaitForSeconds(1f);
            }

            continueText.DOFade(1, 0.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            callback?.Invoke();
            _isClickAvaiable = true;
        }

        public void StopRewardView()
        {
            _isClickAvaiable = false;
            _rewardSequence.Kill();
            continueText.DOKill();
            continueText.DOFade(0, 0.1f).SetEase(Ease.Linear);
            _rewardSequence = DOTween.Sequence();
            _rewardSequence.Append(rewardPanel.transform.DOScale(0, 0.1f).SetEase(Ease.OutQuad));
            _rewardSequence.AppendCallback(() =>
            {
                rewardPanel.SetActive(false);
                gameObject.SetActive(false);
            });
        }

        private void SetRotateLight()
        {
            lightObj.transform.DORotate(new Vector3(0f, 0f, -90f), 1f).SetRelative(true).SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental);
        }
    }
}