using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Match_3
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Current;

        [ShowInInspector] public AudioClip[] audioClips;

        [SerializeField] private AudioSource sfxAudioSource;
        [SerializeField] private AudioSource mbgAudioSource;

        private void Awake()
        {
            if (Current != null) Destroy(gameObject);
            else
            {
                Current = this;
                DontDestroyOnLoad(this);
                PlayMusicBackground();
            }
        }

        public void PlaySound()
        {
            sfxAudioSource.clip = audioClips[0];
            sfxAudioSource.PlayOneShot(audioClips[0]);
        }

        public void PlayMusicBackground()
        {
            mbgAudioSource.DOFade(0.3f, 10).OnStart(SetDefaultVolume);
        }
        
        private void SetDefaultVolume()
        {
            mbgAudioSource.volume = 0f;
        }
    }
}