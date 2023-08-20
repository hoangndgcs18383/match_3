using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Match_3
{
    public class ButtonHandler : Button
    {
        private Sequence _sequence;
        private RectTransform _rectTransform;

        protected override void Awake()
        {
            base.Awake();
            _rectTransform = GetComponent<RectTransform>();
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            switch (state)
            {
                case SelectionState.Normal:
                    _sequence?.Kill();
                    _sequence = DOTween.Sequence();
                    _sequence.Append(_rectTransform.DOScale(1f, 0.1f));
                    _sequence.Append(_rectTransform.DOScale(1.1f, 0.1f));
                    _sequence.Append(_rectTransform.DOScale(1f, 0.1f));
                    break;
                case SelectionState.Highlighted:
                case SelectionState.Pressed:
                    _sequence?.Kill();
                    _sequence = DOTween.Sequence();
                    _sequence.Append(_rectTransform.DOScale(1f, 0.1f));
                    _sequence.Append(_rectTransform.DOScale(0.9f, 0.1f));
                    _sequence.Append(_rectTransform.DOScale(1f, 0.1f));
                    break;
                case SelectionState.Disabled:
                    break;
            }

            base.DoStateTransition(state, instant);
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            _sequence?.Kill();
        }
    }
}