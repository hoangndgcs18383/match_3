using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Match_3
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Current;

        [ShowInInspector] public AudioClip[] audioClips;

        [SerializeField] private float volume = 0.3f;
        [SerializeField] private float duration = 10f;
        
        [SerializeField] private AudioSource sfxAudioSource;
        [SerializeField] private AudioSource mbgAudioSource;

        private void Awake()
        {
            if (Current == null)
            {
                Current = this;
                
                if (transform.parent == null)
                {
                    
                    DontDestroyOnLoad(this);
                }
            }
        }

        private void Start()
        {
            PlayMusicBackground();
        }

        public void PlaySound()
        {
            sfxAudioSource.clip = audioClips[0];
            sfxAudioSource.PlayOneShot(audioClips[0]);
        }

        public void PlayMusicBackground()
        {
            mbgAudioSource.DOFade(volume, duration * 2).OnStart(SetDefaultVolume);
        }
        
        public void StopMusicBackground()
        {
            mbgAudioSource.DOFade(0f, duration).OnStart(SetDefaultVolume);
        }
        
        private void SetDefaultVolume()
        {
            mbgAudioSource.volume = 0f;
        }
    }
}