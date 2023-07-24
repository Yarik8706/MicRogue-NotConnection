using System.Collections;
using MainScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RoomObjects
{
    public class EmergencyExit : MonoBehaviour
    {
        private IEnumerator OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) yield break;
            yield return new WaitUntil(() => GameManager.player.isTurnOver);
            if (GameManager.player.transform.position != transform.position) yield break;
            GameManager.player.isActive = false;
            GameManager.instance.screenFader.fadeState = ScreenFader.FadeState.In;
            yield return new WaitForSeconds(2f);
            PlayerPrefsSafe.SetInt("IsWin", 1);
            SceneManager.LoadScene(0);
        }
    }
}