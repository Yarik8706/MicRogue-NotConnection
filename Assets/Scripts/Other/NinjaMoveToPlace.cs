using System;
using System.Collections;
using MainScripts;
using UnityEngine;

namespace Other
{
    public class NinjaMoveToPlace : MoveToPlace
    {
        [SerializeField] private GameObject movementEffect;
        
        protected override void Active()
        {
            StartCoroutine(ActiveCoroutine());
        }

        private IEnumerator ActiveCoroutine()
        {
            GameManager.player.moveToPlaces.Remove(gameObject);
            GameManager.player.DeleteAllMoveToPlaces();
            yield return new WaitForSeconds(0.6f);
            Instantiate(movementEffect, GameManager.player.transform.position, Quaternion.identity);
            GameManager.player.transform.position = transform.position;
            Instantiate(movementEffect, transform.position, Quaternion.identity);
            GameManager.player.TurnOver();
            Destroy(gameObject);
        }
    }
}