using System;
using Enemies;
using MainScripts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoomObjects
{
    public class BrokenWall : MonoBehaviour, IFireAttack, IClickToAvailablePosition
    {
        public Sprite brokenWallIdleSprite;
        public Sprite brokenWallCanClickSprite;
        private bool _isActive;
        private SpriteRenderer _spriteRenderer;
        private Vector3 _positionForStart;
        
        private void Start()
        {
            GameplayEventManager.OnNextMove.AddListener(NoClickEvent);
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void FireDamage(GameObject firePrefab)
        {
            _positionForStart = Instantiate(firePrefab, transform.position, Quaternion.identity).transform.position;
            _isActive = true;
            _spriteRenderer.sprite = brokenWallIdleSprite;
        }

        public void FireDamage()
        {
            // _isActive = true;
            // _spriteRenderer.sprite = brokenWallIdleSprite;
        }

        public void ClickEvent(GameObject moveToPlace, Player player)
        {
            if(!_isActive) return;
            _spriteRenderer.sprite = brokenWallCanClickSprite;           
            var newObject = Instantiate(moveToPlace, transform.position, Quaternion.identity);
            _positionForStart = newObject.transform.position;
            player.moveToPlaces.Add(newObject);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.GetComponent<Player>() is not { } player) return;
            if (player.transform.position == _positionForStart)
            {
                StartCoroutine(GameManager.instance.NextRoom(ExitLocation.Up));
            }
        }

        private void NoClickEvent()
        {
            if(_isActive) _spriteRenderer.sprite = brokenWallIdleSprite;
        }
    }
}
