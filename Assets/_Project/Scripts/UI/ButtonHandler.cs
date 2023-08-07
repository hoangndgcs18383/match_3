using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Match_3
{
    public class ButtonHandler : Button
    {
        public bool IsMultiTouch = true;
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (InputStatic.TouchCount > 0 && !IsMultiTouch) return;

            base.OnPointerClick(eventData);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            InputStatic.TouchCount++;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            InputStatic.TouchCount--;
        }
    }
}