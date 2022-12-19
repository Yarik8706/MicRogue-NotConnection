using System.Collections;
using MainScripts;
using RoomControllers;
using UnityEngine;

namespace RoomObjects
{
    public class ExitFromShip : MonoBehaviour, IRoomAddiction
    {
        public GameObject mapTarget;
        
        private IEnumerator OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Player>() is not {} player) yield break;
            yield return new WaitUntil(() => player.transform.position == transform.position);
            GameManager.instance.StartWin();
        }

        private void Start()
        {
            Instantiate(mapTarget, RoomController.mapObject.transform.position, Quaternion.identity);
        }

        public RoomController RoomController { get; set; }
    }
}