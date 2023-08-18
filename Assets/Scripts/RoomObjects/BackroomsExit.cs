using System.Collections;
using MainScripts;
using UnityEngine;

namespace RoomObjects
{
    public class BackroomsExit : MonoBehaviour
    {
        private IEnumerator OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) yield break;
            yield return new WaitUntil(() => GameManager.player.isTurnOver);
            if (GameManager.player.transform.position != transform.position) yield break;
            GameManager.player.isActive = false;
            yield return new WaitForSeconds(1f);
            CoroutineController.instance.StartCoroutine(
                GameManager.instance.backroomsController.ExitBackrooms());
        }
    }
}