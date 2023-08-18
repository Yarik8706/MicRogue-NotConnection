using System;
using System.Collections;
using MainScripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Other
{
    public class MusicController : MonoBehaviour
    {
        private AudioClip[] activeAudioClips;
        [SerializeField] private AudioClip backroomsClip;
        [SerializeField] private AudioClip[] baseClips;
        [SerializeField] private AudioClip[] middleClips;
        [SerializeField] private AudioClip[] hardClips;
        [SerializeField] private AudioClip[] veryHardClips;
        [SerializeField] private AudioClip[] trainingClips;
        
        public AudioSource audioSource { get; private set; }

        public static MusicController instance;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            GameplayEventManager.OnNextRoom.AddListener(SetCenterMusic);
        }
        
        private void Start()
        {
            if(instance == this) return;
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            SetCenterMusic();
            StartCoroutine(PlayMusicCoroutine());
            DontDestroyOnLoad(gameObject);
        }

        private void SetCenterMusic()
        {
            if (GameManager.instance == null)
            {
                activeAudioClips = baseClips;
                return;
            }
            switch (GameManager.instance.activeRoomController.roomType)
            {
                case RoomType.Base:
                    activeAudioClips = baseClips;
                    break;
                case RoomType.Middle:
                    activeAudioClips = middleClips;
                    break;
                case RoomType.Hard:
                    activeAudioClips = hardClips;
                    break;
                case RoomType.VeryHard:
                    activeAudioClips = veryHardClips;
                    break;
                case RoomType.Start:
                    activeAudioClips = baseClips;
                    break;
                case RoomType.Training1:
                    activeAudioClips = trainingClips;
                    break;
                case RoomType.Backrooms:
                    activeAudioClips = new[] {backroomsClip};
                    break;
                case RoomType.BackroomsEnd:
                    activeAudioClips = new[] {backroomsClip};
                    break;
            }
        }

        public void PlayBackroomsMusic()
        {
            StopMusicAndPlayNext();
        }

        public void StopMusicAndPlayNext()
        {
            StopCoroutine(PlayMusicCoroutine());
            StartCoroutine(PlayMusicCoroutine());
        }

        private IEnumerator PlayMusicCoroutine()
        {
            var randomClip = activeAudioClips.Length == 1 ? activeAudioClips[0] 
                : activeAudioClips[Random.Range(0, activeAudioClips.Length)];
            audioSource.clip = randomClip;
            audioSource.Play();
            yield return new WaitForSeconds(randomClip.length);
            audioSource.Stop();

            StartCoroutine(PlayMusicCoroutine());
        }
    }
}