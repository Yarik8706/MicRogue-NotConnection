using System.Collections;
using Canvas;
using MainScripts;
using Other;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RoomObjects
{
    public class TheEndObj : MonoBehaviour
    {
        private IEnumerator OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) yield break;
            yield return new WaitUntil(() => GameManager.player.isTurnOver);
            if (GameManager.player.transform.position != transform.position) yield break;
            GameManager.player.isActive = false;
            DialogController.instance.StartDialog(DialogController.instance.endTextDialog, i =>
            {
                if (i == DialogController.instance.endTextDialog.Length)
                {
                    GameManager.instance.screenFader.fadeState = ScreenFader.FadeState.In;
                }

                if (i == -1)
                {
                    SceneManager.LoadScene(0);
                }
            });
        }
    }
}