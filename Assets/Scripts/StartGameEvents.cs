using System;
using System.Collections;
using Canvas;
using MainScripts;
using UnityEngine;

public class StartGameEvents : MonoBehaviour
{
    public GameObject spaceCapsule;
        
    private void Start()
    {
        StartCoroutine(StartEventsCoroutine());
    }

    private IEnumerator StartEventsCoroutine()
    {
        var isStart = false;
        DialogController.instance.StartDialog(DialogController.instance.startTextDialog, i =>
        {
            if (i == -1)
            {
                isStart = true;
            }
        });
        yield return new WaitUntil(() => isStart);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(GameManager.cameraShake.Shake(0.5f, 0.5f));
        spaceCapsule.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        GameManager.player.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(GameManager.instance.TurnStarted());
    }
}