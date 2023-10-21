using System.Collections;
using PlayersScripts;
using UnityEngine;

namespace Multiplayer
{
    public class MultiplayerFinish : MonoBehaviour
    {
        private IEnumerator OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.TryGetComponent(out Player player)) yield break;
            yield return new WaitUntil(() => player.isTurnOver);
            if (player.transform.position != transform.position) yield break;
            yield return null;
            yield return new WaitUntil(() => !player.isTurnOver);
            if (player == null) yield break;
            if (player.transform.position == transform.position)
            {
                MultiplayerGameManager.instance.PlayerWin(player.GetComponent<MultiplayerPlayerController>().colorIndex);
            }
        }
    }
}