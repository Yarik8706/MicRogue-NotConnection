using System;
using System.Collections;
using MainScripts;
using UnityEngine;

namespace RoomObjects
{
    public enum ExitLocation
    {
        Up,
        Right,
        Down,
        Left
    }
    
    public class Exit : MonoBehaviour
    {
        public ExitLocation exitLocation;
        
        [SerializeField] private Sprite rightSprite;
        [SerializeField] private Sprite leftSprite;

        private SpriteRenderer _spriteRenderer;
        internal bool isActive = true;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private IEnumerator OnTriggerEnter2D(Collider2D other)
        {
            if (!isActive) yield break;
            if (!other.gameObject.CompareTag("Player")) yield break;
            yield return new WaitUntil(() => GameManager.player.isTurnOver);
            if (GameManager.player.transform.position != transform.position) yield break;
            if (GameManager.player == null) yield break;
            if (GameManager.player.transform.position == transform.position)
            {
                StartCoroutine(GameManager.instance.NextRoom(exitLocation));
            }
        }

        public void SetDirectionAndSpriteByDirection(ExitLocation direction)
        {
            exitLocation = direction;
            _spriteRenderer.sprite = direction == ExitLocation.Right ? rightSprite : leftSprite;
        }

        public Vector3 GetNextPositionPlayer()
        {
            return new Vector3(transform.position.x, transform.position.y);
        }
    }
}
