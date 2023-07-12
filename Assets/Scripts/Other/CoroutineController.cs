using System;
using System.Collections;
using UnityEngine;

public class CoroutineController : MonoBehaviour
{
    public static CoroutineController instance;

    private void Awake()
    {
        instance = this;
    }

    public IEnumerator CoroutineWithCallback(IEnumerator coroutine, Action callback)
    {
        yield return coroutine;
        callback.Invoke();
    }
}