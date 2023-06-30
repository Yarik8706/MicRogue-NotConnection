using System;
using System.Collections;
using System.Linq;
using MainScripts;
using Unity.VisualScripting;
using UnityEngine;

namespace RoomObjects
{
    public class SlimeTrap : MonoBehaviour
    {
        private Animator _animator;
        
        [SerializeField] private int freezingWaitCount;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public void Initializate(TheEssence essence)
        {
            StartCoroutine(InitializateCoroutine(essence));
        }
        
        public IEnumerator InitializateCoroutine(TheEssence essence)
        {
            essence.essenceEffect = TheEssenceEffect.Freezing;
            var baseVariantsPositions = essence.variantsPositions;
            Vector2[] newVariantsPositions;
            if (essence is Player)
            { 
                newVariantsPositions = baseVariantsPositions.Where(
                   variantPosition => Vector2.Distance(variantPosition, new Vector2(0, 0)) < 2).ToArray();
                essence.variantsPositions = newVariantsPositions;
            }
            else
            {
                newVariantsPositions = new Vector2[] { };
                essence.variantsPositions = newVariantsPositions;
                var numberOfMovesNow = GameManager.numberOfMoves;
                yield return new WaitUntil(() => numberOfMovesNow+freezingWaitCount == GameManager.numberOfMoves);
                essence.variantsPositions = baseVariantsPositions;
            }
            yield return new WaitUntil(() => essence.isMove);
            _animator.SetTrigger("Explosion");
            essence.variantsPositions = baseVariantsPositions;
            essence.essenceEffect = TheEssenceEffect.None;
        }
    }
}