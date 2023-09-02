using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;

namespace Match_3
{
    public class TimerManager : MonoBehaviour
    {
        private static TimerManager _current;
        public static TimerManager Current => _current;

        private float _countDownTime;
        private string _countString;

        public Action<float> CountDownCallback;
        private CoroutineHandle _countDownCoroutine;

        private void Awake()
        {
            if (_current == null)
            {
                _current = this;
                DontDestroyOnLoad(this);
            }
        }

        private void Start()
        {
            StartCountDown();
        }

        private float Time => ProfileDataService.Instance.GetLastTimeReceiveLife();

        public void StartCountDown()
        {
            Timing.KillCoroutines(_countDownCoroutine);
            _countDownTime = Time;
            Debug.Log($"Time: {_countDownTime}");

            if (_countDownTime <= 0)
            {
                _countDownTime = 0;
                CountDownCallback?.Invoke(_countDownTime);
                return;
            }

            _countDownCoroutine = Timing.RunCoroutine(CountDown());
        }

        private IEnumerator<float> CountDown()
        {
            while (_countDownTime > 0)
            {
                _countDownTime -= 1;
                CountDownCallback?.Invoke(_countDownTime);
                yield return Timing.WaitForSeconds(1f);
            }
        }


        public void StopCountDown()
        {
            Timing.KillCoroutines(_countDownCoroutine);
            ProfileDataService.Instance.SetLastTimeReceiveLife((int)_countDownTime);
        }
    }
}