using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Match_3
{
    public class FlyTextManager : MonoBehaviour
    {
        public static FlyTextManager Instance;

        [SerializeField] private TMP_Text flyTextPrefab;

        private Queue<string> _flyTextQueue = new Queue<string>();

        private Sequence sequence;
        
        private void Awake()
        {
            Instance = this;
        }

        public void SetFlyText(string text)
        {
            sequence?.Kill();
            _flyTextQueue.Enqueue(text);
            ShowFlyText();
        }

        private void ShowFlyText()
        {
            flyTextPrefab.SetText(_flyTextQueue.Dequeue());

            flyTextPrefab.gameObject.SetActive(true);
            flyTextPrefab.transform.localScale = Vector3.zero;
            sequence = DOTween.Sequence();
            sequence.Append(flyTextPrefab.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
            sequence.Join(flyTextPrefab.transform.DOLocalMoveY(100, 0.5f).SetEase(Ease.OutBack));
            sequence.Append(flyTextPrefab.transform.DOLocalMoveY(200, 0.5f).SetEase(Ease.InBack));
            sequence.Append(flyTextPrefab.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack));
            sequence.OnComplete(() =>
            {
                if (_flyTextQueue.Count > 0)
                {
                    ShowFlyText();
                }
                else
                {
                    flyTextPrefab.gameObject.SetActive(false);
                }
            });
        }

        private void OnDisable()
        {
            sequence?.Kill();
        }

        private void OnDestroy()
        {
            sequence?.Kill();
        }
    }
}