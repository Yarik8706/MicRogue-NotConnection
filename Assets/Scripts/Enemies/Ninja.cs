using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class Ninja : TheEnemy
    {
        public string moveAnimationName2;
        private Vector3 _nextPosition;

        protected override IEnumerator AttackPlayer(Vector2 playerPosition)
        {
            yield return Move(playerPosition);
        }

        public override IEnumerator Move(Vector3 @where)
        {
            animator.SetTrigger(moveAnimationName);
            baseAnimations = Instantiate(baseAnimationsObj, transform.position, Quaternion.identity)
                .GetComponent<BaseAnimations>();
            baseAnimations.isDied = false;
            baseAnimations.DiedAnimation();
            _nextPosition = @where;
            yield return null;
        }

        public void MoveEnd()
        {
            transform.position = _nextPosition;
            baseAnimations.gameObject.transform.position = _nextPosition;
            baseAnimations.isDied = true;
            baseAnimations.DiedAnimation();
            animator.SetTrigger(moveAnimationName2);
            TurnOver();
        }

        public static bool CheckEmptyPlace(Vector2 checkingPosition, LayerMask blockingLayer)
        {
            var hit = Physics2D.Linecast(new Vector2(checkingPosition.x-0.1f, checkingPosition.y), checkingPosition, blockingLayer);
            return hit.collider == null;
        }

        protected override Vector2[] MoveCalculation(Vector2[] theVariantsPositions)
        {
            var nowVariantsPositions = new List<Vector2> {Capacity = 0};
            foreach (var newVariantPosition in theVariantsPositions)
            {
                if (CheckEmptyPlace(newVariantPosition, blockingLayer))
                {
                    nowVariantsPositions.Add(newVariantPosition);
                }
            }
            return nowVariantsPositions.ToArray();
        }
    }
}
