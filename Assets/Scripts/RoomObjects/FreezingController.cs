using System;
using System.Collections;
using MainScripts;
using UnityEngine;

namespace RoomObjects
{
    public class FreezingController : MonoBehaviour
    {
        private Animator _animator;
        [SerializeField] private Color freezingColor = Color.cyan;
        [SerializeField] private int freezingWaitCount;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Initializate(TheEssence essence)
        {
            StartCoroutine(InitializateCoroutine(essence));
        }

        private IEnumerator InitializateCoroutine(TheEssence essence)
        {
            essence.essenceEffect = TheEssenceEffect.Freezing;
            transform.SetParent(essence.transform);
            essence.isActive = false;
            var baseAnimatorSpeed = essence.animator.speed;
            var baseColorSprite = essence.spriteRenderer.color;
            essence.spriteRenderer.color = freezingColor;
            essence.animator.speed = 0;
            var numbersOfMovesNow = GameManager.numberOfMoves;
            yield return new WaitUntil(() => numbersOfMovesNow+freezingWaitCount == GameManager.numberOfMoves);
            essence.isActive = true;
            transform.parent = null;
            yield return new WaitUntil(() => essence.isMove);
            essence.animator.speed = baseAnimatorSpeed;
            essence.spriteRenderer.color = baseColorSprite;
            _animator.SetTrigger("Frosbite");
            essence.essenceEffect = TheEssenceEffect.None;
        }
    }
}