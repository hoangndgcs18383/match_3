using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Match_3
{
    public class MonsterController : MonoBehaviour
    {
        public Animator animator;
        public SpriteRenderer spriteRenderer;
        public float speed = 1f;
        public float timeDelay = 10f;
        public float timeDelayMin = 0.5f;
        public float timeDelayMax = 0.5f;

        private const string Walking = "isWalking";
        private const string Dead = "Dead";
        private Transform _transform;

        private Vector3 _targetPosition = new Vector3(-7, 0, 0);
        private Vector3 _direction = Vector3.zero;
        private float _timeDelay;

        private void Start()
        {
            Initialized();
        }

        public void Initialized()
        {
            _timeDelay = timeDelay;
            _transform = transform;
        }

        private void Update()
        {
            if (Vector3.Distance(_transform.position, _targetPosition) > 0.1f)
            {
                var position = _transform.position;
                _direction = (_targetPosition - position).normalized;
                UpdateRotation();
                position = Vector3.MoveTowards(position, _targetPosition, speed * Time.deltaTime);
                _transform.position = position;
                animator.SetBool(Animator.StringToHash(Walking), true);
            }
            else
            {
                if (_timeDelay < 0)
                {
                    _timeDelay = Random.Range(timeDelayMin, timeDelayMax);
                    _targetPosition = new Vector3(-_targetPosition.x, RandomY(), 0);
                }

                animator.SetBool(Animator.StringToHash(Walking), false);

                _timeDelay -= Time.deltaTime;
            }
        }

        private float RandomY()
        {
            return Random.Range(-3f, 3f);
        }

        private void UpdateRotation()
        {
            spriteRenderer.flipX = _direction.x > 0;
        }

        private void RandomJump()
        {
            var random = Random.Range(0, 2);
            if (random == 0)
            {
                _transform.DOJump(_transform.position, 0.5f, 1, 0.5f).SetEase(Ease.OutSine);
            }
        }

        private void OnMouseEnter()
        {
            _transform.DOScale(0.9f, 0.1f).SetEase(Ease.OutSine);
        }
        
        private void OnMouseExit()
        {
            _transform.DOScale(1f, 0.1f).SetEase(Ease.OutSine);
        }
    }
}