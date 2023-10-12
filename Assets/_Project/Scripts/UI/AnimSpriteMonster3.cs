using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Match_3
{
    public class AnimSpriteMonster3 : MonoBehaviour
    {
        [SerializeField] private float duration;
        [SerializeField] private float offsetY;
        [SerializeField] private Button button;

        private RectTransform _rectTransform;
        private Sequence _sequence;
        private Vector3 _originPos;

        private void Start()
        {
            button.onClick.AddListener(OnButtonClicked);
            button.interactable = false;
            PlayAnim();
        }

        private void OnButtonClicked()
        {
            button.interactable = false;
            _rectTransform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
            {
                _rectTransform.DOScale(Vector3.one, 0.5f);
            });
            _sequence.Kill();
            _rectTransform.DOKill();
            _rectTransform.DOLocalMoveY(_originPos.y, duration);
            _rectTransform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);
            
            float randomPlay = UnityEngine.Random.Range(20f, 30f);
            Invoke(nameof(PlayAnim), randomPlay);
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
            button.interactable = false;
            _sequence.OnComplete(() =>
            {
                button.interactable = true;
            });
            /*_sequence.Append(_rectTransform.DOLocalMoveX(-_originPos.x, duration));
            _sequence.Append(_rectTransform.DOLocalMoveX(_originPos.x, duration));
            _sequence.Append(_rectTransform.DOLocalMoveY(-offsetY, duration).SetRelative(true));*/
        }
        
        
        
    }
}