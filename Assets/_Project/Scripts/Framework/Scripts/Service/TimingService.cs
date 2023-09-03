using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zeff.Core.Service;

namespace Match_3
{
    public enum UpdateType
    {
        Update,
        FixedUpdate,
        LateUpdate
    }

    public class TimingService : Service<TimingService>, IDisposable
    {
        private UpdateType _updateType;
        

        private Queue<Timer> _timers = new Queue<Timer>();

        public override void Initialize()
        {
            base.Initialize();
        }

        public void AddTimer(float time, Action callback, UpdateType updateType = UpdateType.Update)
        {
            _timers.Enqueue(new Timer(time, callback));
        }
        
        public void AddTimer(float time, Action callback, int repeat, RepeatTimer.RepeatType repeatType = RepeatTimer.RepeatType.Repeat, UpdateType updateType = UpdateType.Update)
        {
            _timers.Enqueue(new RepeatTimer(time, callback, repeat, repeatType));
        }

        public void Update()
        {
            foreach (var timerUpdate in _timers)
            {
                timerUpdate.Update();
            }
        }

        public void FixedUpdate()
        {
            return;

        }

        public void LateUpdate()
        {
            return;

        }

        public void ResetAll()
        {
            foreach (var timerUpdate in _timers)
            {
                timerUpdate.Reset();
            }
        }

        public void Dispose()
        {
            foreach (var timerUpdate in _timers)
            {
                timerUpdate.Reset();
            }
            
            _timers.Clear();
        }
    }

    public class RepeatTimer : Timer
    {
        public enum RepeatType
        {
            Repeat,
            RepeatForever
        }
        
        private int _repeat;
        private RepeatType _repeatType;
        private float _loopTime;

        public RepeatTimer(float time, Action callback, int repeat, RepeatType repeatType = RepeatType.Repeat) : base(time, callback)
        {
            _currentTime = time;
            _loopTime = time;
            _callback = callback;
            _repeat = repeat;
            _repeatType = repeatType;

            if (time <= 0)
            {
                _callback?.Invoke();
                Reset();
            }
        }
        
        protected override void OnTimerComplete()
        {
            _callback?.Invoke();
            
            if (_repeatType == RepeatType.RepeatForever || _repeat < 0)
            {
                _currentTime = _loopTime;
                return;
            }
            
            _repeat--;
            if (_repeat <= 0)
            {
                Reset();
            }
            else
            {
                _currentTime = _loopTime;
            }
        }
    }

    public class Timer
    {
        protected float _currentTime;
        protected Action _callback;

        public Timer(float time, Action callback)
        {
            _currentTime = time;
            _callback = callback;

            if (time <= 0)
            {
                _callback?.Invoke();
                Reset();
            }
        }

        public virtual void Update()
        {
            _currentTime -= Time.deltaTime;
            if (_currentTime <= 0)
            {
                OnTimerComplete();
            }
        }
        
        protected virtual void OnTimerComplete()
        {
            _callback?.Invoke();
            Reset();
        }

        public virtual void Reset()
        {
            _currentTime = 0;
            _callback = null;
        }
    }
}