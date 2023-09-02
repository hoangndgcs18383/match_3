
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils
{
    public class DragHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public delegate void OnDragEndHandler(Vector2 pos);

        public event OnDragEndHandler OnDragEnd;

        private RectTransform _current;

        private Vector2 _newPos;
        private Vector2 _offsetMin;
        private Vector2 _offsetMax;

        private Vector2 _delta;

        private void Awake()
        {
            var tf = transform;
            _current = (RectTransform)tf;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _delta = ((Vector3)eventData.position - _current.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _current.position = NewPosition(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnDragEnd?.Invoke(_current.position);
        }

        private Vector2 NewPosition(Vector2 mousePos)
        {
            _newPos = mousePos - _delta;
            Rect rect = _current.rect;
            Vector3 lossyScale = _current.root.lossyScale;

            Vector2 distance = new Vector2(rect.width * lossyScale.x, rect.height * lossyScale.y);

            Vector2 pivot = _current.pivot;
            _offsetMin.x = _newPos.x - pivot.x * distance.x;
            _offsetMin.y = _newPos.y - pivot.y * distance.y;
            _offsetMax.x = _newPos.x + (1 - pivot.x) * distance.x;
            _offsetMax.y = _newPos.y + (1 - pivot.y) * distance.y;

            if (_offsetMin.x < 0)
            {
                _newPos.x = _current.pivot.x * distance.x;
            }
            else if (_offsetMax.x > Screen.width)
            {
                _newPos.x = Screen.width - (1 - _current.pivot.x) * distance.x;
            }

            if (_offsetMin.y < 0)
            {
                _newPos.y = _current.pivot.y * distance.y;
            }
            else if (_offsetMax.y > Screen.height)
            {
                _newPos.y = Screen.height - (1 - _current.pivot.y) * distance.y;
            }

            return _newPos;
        }
    }
}