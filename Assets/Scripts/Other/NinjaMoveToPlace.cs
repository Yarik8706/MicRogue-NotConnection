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
            player.moveToPlaces.Remove(gameObject);
            player.DeleteAllMoveToPlaces();
            yield return new WaitForSeconds(0.6f);
            Instantiate(movementEffect, player.transform.position, Quaternion.identity);
            player.transform.position = transform.position;
            Instantiate(movementEffect, transform.position, Quaternion.identity);
            player.TurnOver();
            Destroy(gameObject);
        }
    }
}