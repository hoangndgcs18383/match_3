using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Match_3
{
    public static class UIAnimationUtils
    {
        
        public static void Spin(this Transform transform)
        {
            Sequence sequence = DOTween.Sequence();
            DOTween.Kill(transform);
            
            sequence.Insert(0f, transform.DOPunchPosition(new Vector3(5f, 5f, 0.1f), Random.Range(1f, 2f), 5, 1).SetEase(Ease.InBounce));
            sequence.Insert(0f, transform.DOPunchRotation(new Vector3(0f, 0f, 10f), Random.Range(1f, 2f), 5, 1).SetEase(Ease.InOutBack));
            sequence.Insert(0f, transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), Random.Range(1f, 2f), 5, 1).SetEase(Ease.InBounce));
            sequence.Insert(2f, transform.DOScale(1.1f, Random.Range(0.5f, 1f)).SetEase(Ease.Linear).SetLoops(Random.Range(2, 4), LoopType.Yoyo));
            sequence.SetLoops(-1, LoopType.Yoyo);
        }

        public static void DrillScale(this Transform transform)
        {
            Sequence sequence = DOTween.Sequence();
            DOTween.Kill(transform);
            
            //sequence.Insert(0f, transform.DOPunchPosition(new Vector3(5f, 5f, 0.1f), Random.Range(1f, 2f), 5, 1).SetEase(Ease.InBounce));
            //sequence.Insert(0f, transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), Random.Range(1f, 2f), 5, 1).SetEase(Ease.InBounce));
            sequence.Insert(2f, transform.DOScale(1.1f, Random.Range(0.5f, 1f)).SetEase(Ease.Linear).SetLoops(Random.Range(2, 4), LoopType.Yoyo));
            sequence.SetLoops(-1, LoopType.Yoyo);
        }

        public static void DOTextPunchY(this Transform transform)
        {
            Sequence sequence = DOTween.Sequence();
            DOTween.Kill(transform);
            
            sequence.Insert(0f, transform.DOPunchPosition(new Vector3(0f, 10f, 0.1f), Random.Range(1f, 2f), 5, 1).SetEase(Ease.InBounce));
            sequence.SetLoops(-1, LoopType.Yoyo);
        }
    }
}
