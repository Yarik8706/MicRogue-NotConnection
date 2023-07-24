using System;
using System.Collections;
using System.Linq;
using MainScripts;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace RoomObjects
{
    public class UpdateChest : MonoBehaviour, IClickToAvailablePosition, IPointerClickHandler
    {
        [SerializeField] private GameObject mimicChest;
        [SerializeField] private int spawnMimicChance;
        [SerializeField] private GameObject updateShieldEffect;

        private GameObject _moveToPlace;

        private void Start()
        {
            GameplayEventManager.OnNextRoom.AddListener(() =>
            {
                Destroy(gameObject);
            });
        }

        private IEnumerator OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) yield break;
            yield return new WaitUntil(() => GameManager.player.isTurnOver);
            if (GameManager.player.transform.position != transform.position) yield break;
            GameManager.player.RestorationOfShields();
            Destroy(gameObject);
        }
        
        public void ClickEvent(GameObject moveToPlace, Player player)
        {
            var newObject = Instantiate(moveToPlace, transform.position, Quaternion.identity);
            newObject.GetComponent<MoveToPlace>().isActive = false;
            player.moveToPlaces.Add(newObject);
            _moveToPlace = newObject;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(_moveToPlace == null) return;
            StartCoroutine(ActiveCoroutine());
        }

        private IEnumerator ActiveCoroutine()
        {
            var randomNumber = Random.Range(0, spawnMimicChance);
            if (randomNumber == 0)
            {
                yield return GameManager.player.Move(transform.position);
                GameManager.player.ResetConsumables();
                Instantiate(updateShieldEffect, transform.position, Quaternion.identity);
            }
            else
            {
                Destroy(_moveToPlace);
                var moveToPlaces = GameManager.player.moveToPlaces.Where(o => o != null).Select(select => select.GetComponent<MoveToPlace>());
                foreach (var moveToPlace in moveToPlaces)
                {
                    moveToPlace.isActive = false;
                }
                yield return new WaitForSeconds(0.5f);
                Instantiate(mimicChest, transform.position, Quaternion.identity);
                GetComponent<SpriteRenderer>().enabled = false;
                yield return new WaitForSeconds(1f);
                foreach (var moveToPlace in moveToPlaces)
                {
                    moveToPlace.isActive = true;
                }
            }
            Destroy(gameObject);
        }
    }
}
