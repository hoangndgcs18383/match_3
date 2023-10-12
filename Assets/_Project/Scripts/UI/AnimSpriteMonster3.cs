using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Match_3
{
    public class AnimSpriteMonster3 : MonoBehaviour
    {
        [SerializeField] private float duration;
        [SerializeField] private float offsetY;

        private RectTransform _rectTransform;
        private Sequence _sequence;
        private Vector3 _originPos;

        private void Start()
        {
            PlayAnim();
        }

        [Button]
        public void PlayAnim()
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();

            _originPos = _rectTransform.localPosition;

            _sequence = DOTween.Sequence();
            _sequence.Append(_rectTransform.DOLocalMoveY(offsetY, duration).SetRelative(true));
            _rectTransform.DOLocalRotate(new Vector3(0, 0, 2), 1).SetLoops(-1, LoopType.Yoyo);
            /*_sequence.Append(_rectTransform.DOLocalMoveX(-_originPos.x, duration));
            _sequence.Append(_rectTransform.DOLocalMoveX(_originPos.x, duration));
            _sequence.Append(_rectTransform.DOLocalMoveY(-offsetY, duration).SetRelative(true));*/
        }
    }
}