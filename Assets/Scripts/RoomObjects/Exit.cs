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
        public bool willNotActive;
        internal bool isActive = true;

        private IEnumerator OnTriggerEnter2D(Collider2D other)
        {
            if (!isActive) yield break;
            if (!other.gameObject.CompareTag("Player")) yield break;
            yield return new WaitUntil(() => GameManager.player.isTurnOver);
            if (GameManager.player.transform.position != transform.position) yield break;
            yield return null;
            yield return new WaitUntil(() => !GameManager.player.isTurnOver);
            if (GameManager.player == null) yield break;
            if (GameManager.player.transform.position == transform.position)
            {
                StartCoroutine(GameManager.instance.NextRoom(exitLocation));
            }
        }

        public void SetDirectionAndSpriteByDirection(ExitLocation direction)
        {
            exitLocation = direction;
            switch (direction)
            {
                case ExitLocation.Down:
                    transform.Rotate(Vector3.forward*-90);
                    break;
                case ExitLocation.Up:
                    transform.Rotate(Vector3.forward*90);
                    break;
                case ExitLocation.Left:
                    transform.Rotate(-Vector3.forward*180);
                    break;
                case ExitLocation.Right:
                    transform.rotation = Quaternion.identity;
                    break;
            }
        }

        public Vector3 GetNextPositionPlayer()
        {
            return new Vector3(transform.position.x, transform.position.y);
        }
    }
}
