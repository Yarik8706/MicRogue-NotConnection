using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class Ninja : TheEnemy
    {
        public string moveAnimationName2;

        protected override void SelectAction(Vector2 nextPosition)
        {
            if (nextPosition == enemyTargetPosition
                && !CanKillEssence(enemyTarget, nextPosition - Vector2.down * 0.1f))
            {
                TurnOver();
                return;
            }
            base.SelectAction(nextPosition);
        }

        protected override IEnumerator AttackPlayer(TheEssence essence)
        {
            yield return Move(essence.transform.position);
        }

        public override IEnumerator Move(Vector3 @where)
        {
            isMove = true;
            animator.SetTrigger(moveAnimationName);
            Instantiate(diedEffect, transform.position, Quaternion.identity);
            movingPosition = @where;
            yield return null;
        }

        public void MoveEnd()
        {
            transform.position = movingPosition;
            Instantiate(diedEffect, transform.position, Quaternion.identity);
            animator.SetTrigger(moveAnimationName2);
            TurnOver();
        }

        public static bool CheckEmptyPlace(Vector2 checkingPosition, LayerMask blockingLayer, out RaycastHit2D outHit)
        {
            var hit = Physics2D.Linecast(new Vector2(checkingPosition.x-0.1f, checkingPosition.y), checkingPosition, blockingLayer);
            outHit = hit;
            return hit.collider == null;
        }
        
        public static bool CheckEmptyPlace(Vector2 checkingPosition, LayerMask blockingLayer)
        {
            var hit = Physics2D.Linecast(new Vector2(checkingPosition.x-0.1f, checkingPosition.y), checkingPosition, blockingLayer);
            return hit.collider == null;
        }
        
        public static Collider2D GetPlaceObject(Vector2 checkingPosition, LayerMask blockingLayer)
        {
            return Physics2D.Linecast(new Vector2(checkingPosition.x-0.1f, checkingPosition.y), checkingPosition, blockingLayer).collider;
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
