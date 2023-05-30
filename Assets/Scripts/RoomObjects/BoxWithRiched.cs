using System.Collections;
using MainScripts;
using RoomControllers;
using UnityEngine;

namespace RoomObjects
{
    public class BoxWithRiched : MonoBehaviour, IRoomAddiction
    {
        private static int _richedCount;
        public GameObject mapTarget;
        public RoomController RoomController { get; set; }

        private void Start()
        {
            _richedCount++;
            // mapTarget = Instantiate(mapTarget, RoomController.mapObject.transform.position, Quaternion.identity);
        }

        private IEnumerator OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<Player>() is {} player)
            {
                yield return new WaitUntil(() => player.isTurnOver);
                if (player.transform.position != transform.position) yield break;
                _richedCount--;
                Destroy(mapTarget);
                Destroy(gameObject);
            };
        }
    }
}
