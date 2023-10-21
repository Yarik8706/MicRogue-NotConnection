using System.Collections;
using MainScripts;
using PlayersScripts;
using UnityEngine;

namespace RoomObjects
{
    public class FreezingController : MonoBehaviour
    {
        internal Animator animator { get; private set; }
        [SerializeField] private Material freezingMaterial;
        [SerializeField] private int freezingWaitCount;
        [SerializeField] private int freezingWaitCountForPlayer = 2;

        private void Awake()
        {
            animator = GetComponent<Animator>();
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
            essence.spriteRenderer.material = freezingMaterial;
            essence.animator.speed = 0;
            var numbersOfMovesNow = GameManager.numberOfMoves;
            if (essence is Player)
            {
                yield return new WaitUntil(() => numbersOfMovesNow+freezingWaitCountForPlayer == GameManager.numberOfMoves);
            }
            else
            {
                yield return new WaitUntil(() => numbersOfMovesNow+freezingWaitCount == GameManager.numberOfMoves);
            }
            essence.isActive = true;
            transform.parent = null;
            yield return new WaitUntil(() => essence.isMove);
            essence.animator.speed = baseAnimatorSpeed;
            animator.SetTrigger("Frosbite");
            essence.essenceEffect = TheEssenceEffect.None;
        }
    }
}