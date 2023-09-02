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
            _rectTransform.DOKill();
            switch (state)
            {
                case SelectionState.Normal:
                    _rectTransform.DOScale(1f, 0.1f);
                    break;
                case SelectionState.Highlighted:
                    _rectTransform.DOScale(0.9f, 0.1f);
                    break;
                case SelectionState.Pressed:
                    break;
                case SelectionState.Disabled:
                    break;
            }

            base.DoStateTransition(state, instant);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            _rectTransform.DOScale(0.9f, 0.1f);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            _rectTransform.DOScale(1f, 0.1f);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _rectTransform.DOKill();
        }
    }
}