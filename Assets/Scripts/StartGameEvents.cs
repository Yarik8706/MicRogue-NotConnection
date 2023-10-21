using System;
using System.Collections;
using Canvas;
using MainScripts;
using UnityEngine;

public class StartGameEvents : MonoBehaviour
{
    [SerializeField] private GameObject spaceCapsule;
    
    private void Start()
    {
        StartCoroutine(StartEventsCoroutine());
    }

    private IEnumerator StartEventsCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(CameraShake.instance.Shake(0.5f, 0.5f));
        spaceCapsule.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        GameManager.player.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(GameManager.instance.TurnStarted());
    }
}