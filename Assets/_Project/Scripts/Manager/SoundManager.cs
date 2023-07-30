using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Match_3
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Current;

        [ShowInInspector] public AudioClip[] audioClips;
        
        private AudioSource _currentAudioSound;

        private void Awake()
        {
            Current = this;
            DontDestroyOnLoad(this);
            _currentAudioSound = GetComponent<AudioSource>();
        }
        
        public void PlaySound()
        {
            _currentAudioSound.clip = audioClips[0];
            _currentAudioSound.PlayOneShot(audioClips[0]);
        }
    }
}
