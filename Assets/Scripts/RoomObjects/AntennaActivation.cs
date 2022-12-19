using System.Collections;
using MainScripts;
using Other;
using UnityEngine;

namespace RoomObjects
{
    public class AntennaActivation : MonoBehaviour
    {
        public Action1Controller action1Controller;
        
        private IEnumerator OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) yield break;
            yield return new WaitUntil(() => GameManager.player.isTurnOver);
            if (GameManager.player.transform.position != transform.position) yield break;
            GameManager.player.isActive = false;
            action1Controller.StartAction1();
            Destroy(gameObject);
        }
    }
}