using System;
using UnityEditor;
using UnityEngine;

namespace Canvas
{
    public class SoundEffectsController : MonoBehaviour
    {
        private AudioSource _audioSource;

        public static SoundEffectsController instance;

        private void Start()
        {
            instance = this;
            _audioSource = GetComponent<AudioSource>();
        }

        public void StartSound(AudioClip audioClip)
        {
            _audioSource.clip = audioClip;
            _audioSource.Play();
        }
    }
}