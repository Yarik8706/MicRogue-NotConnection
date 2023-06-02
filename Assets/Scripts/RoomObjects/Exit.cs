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
        public bool isActive = true;
        public ExitLocation exitLocation;
        // public Vector3 nextPositionPlayer;
        public Vector3 movingPosition;

        private IEnumerator OnTriggerEnter2D(Collider2D other)
        {
            if (!isActive) yield break;
            if (other.GetComponent<Player>() is not {} player) yield break;
            yield return new WaitUntil(() => GameController.instance.enemiesActive);
            yield return new WaitUntil(() => !GameController.instance.enemiesActive);
            if (GameManager.player == null) yield break;
            if (player.transform.position == movingPosition + transform.position)
            {
                StartCoroutine(GameManager.instance.NextRoom(exitLocation));
            }
        }

        public Vector3 GetNextPositionPlayer()
        {
            return new Vector3(transform.position.x, transform.position.y);
            // return new Vector3(transform.position.x + nextPositionPlayer.x, nextPositionPlayer.y + transform.position.y);
        }
    }
}
