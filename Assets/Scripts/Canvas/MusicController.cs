using System;
using System.Collections;
using UnityEngine;

namespace Other
{
    public class MusicController : MonoBehaviour
    {
        [SerializeField] private AudioClip[] audioClips;
        [SerializeField] private AudioSource audioSource;

        private static MusicController _instance;

        private void Awake()
        {
            if(_instance == this) return;
            if(_instance != null) Destroy(gameObject);
            _instance = this;
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
            StartCoroutine(PlayMusicCoroutine());
        }

        private IEnumerator PlayMusicCoroutine()
        {
            foreach (var audioClip in audioClips)
            {
                audioSource.clip = audioClip;
                audioSource.Play();
                Debug.Log("audioClip.length: " + audioClip.length);
                yield return new WaitForSeconds(audioClip.length);
                audioSource.Stop();
            }
        }
    }
}