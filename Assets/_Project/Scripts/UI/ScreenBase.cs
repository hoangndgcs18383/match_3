using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Match_3
{
    public class ScreenBase : MonoBehaviour
    {
        public bool isInactiveAtAwake = false;        
        public bool IsActivated
        {
            get => gameObject.activeSelf;
            private set => gameObject.SetActive(value);
        }

        private void Awake()
        {
            if(isInactiveAtAwake)
                IsActivated = false;
        }

        protected virtual void OnEnable()
        {
            if (GameManager.Current == null)
            {
                return;
            }
            
            GameManager.Current.GameState = GameState.PAUSE;
        }

        protected virtual void OnDisable()
        {
            if (GameManager.Current == null)
            {
                return;
            }

            GameManager.Current.GameState = GameState.PLAYING;
        }
    }
}