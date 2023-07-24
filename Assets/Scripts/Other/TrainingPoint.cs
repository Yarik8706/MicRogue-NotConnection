using System;
using System.Collections;
using MainScripts;
using UnityEngine;
using UnityEngine.Events;

namespace Other
{
    public class TrainingPoint : MonoBehaviour
    {
        public UnityEvent onPointEvent;

        private void Start()
        {
            TrainingController.instance.trainingPoint = this;
            gameObject.SetActive(false);
        }

        private IEnumerator OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) yield break;
            yield return new WaitUntil(() => GameManager.player.isTurnOver);
            if (GameManager.player.transform.position != transform.position) yield break;
            onPointEvent.Invoke();
        }
    }
}