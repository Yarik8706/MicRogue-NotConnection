using System;
using System.Collections;
using System.Linq;
using MainScripts;
using UnityEngine;

namespace Enemies
{
    public class FlyingEye : TheEnemy
    {
        [SerializeField] private GameObject ray;
        [SerializeField] private GameObject rayNearEssence;
        [SerializeField] private float centerAttackTime;
        [SerializeField] private float endAttackTime;
        
        private static readonly int TriggerAttack = Animator.StringToHash("Attack");

        protected override Vector2 SelectMovePosition(Vector2 playerPosition, Vector2[] theVariantsPositions)
        {
            var centerVariantsPositions = theVariantsPositions.Where(
                variantPosition => transform.position.y - variantPosition.y != 0
                ).ToList();
            var position = SelectionOfTheNearestPosition(
                playerPosition,
                centerVariantsPositions.Count < 2 ? theVariantsPositions : centerVariantsPositions.ToArray(), 
                transform.position);
            return position;
        }

        protected override void SelectAction(Vector2 nextPosition, Vector2 playerPosition)
        {
            if(nextPosition.x < transform.position.x && turnedRight || nextPosition.x > transform.position.x && !turnedRight)
            {
                Flip();
            }
            if (Vector2.Distance(transform.position, playerPosition) <= 1)
            {
                StartCoroutine(base.AttackPlayer(playerPosition));
                return;
            }
            if (Mathf.Abs(transform.position.y - playerPosition.y) < 0.1f)
            {
                boxCollider2D.enabled = false;
                var hit = Physics2D.Linecast(transform.position, playerPosition, blockingLayer);
                boxCollider2D.enabled = true;
                if (hit.collider == null)
                {
                    if(playerPosition.x - transform.position.x > 0 && !turnedRight 
                       || playerPosition.x - transform.position.x < 0 && turnedRight)
                    {
                        Flip();
                    }
                    StartCoroutine(AttackPlayer(playerPosition));
                    return;
                }
            }
            if (nextPosition == (Vector2)transform.position)
            {
                TurnOver();
                return;
            }
            
            StartCoroutine(Move(nextPosition));
        }

        protected override IEnumerator AttackPlayer(Vector2 playerPosition)
        {
            animator.SetTrigger(TriggerAttack);
            yield return ChangingPositionsOfTwoEntities(
                transform,
                GameManager.player.transform,
                endAttackTime,
                centerAttackTime,
                rayNearEssence,
                ray
            );
            TurnOver();
            turnedRight = !(transform.localScale.x > 0);
        }

        public static IEnumerator ChangingPositionsOfTwoEntities(
            Transform mainObject, 
            Transform centerObject, 
            float endAttackTime, 
            float centerAttackTime, 
            GameObject rayNearEssence,
            GameObject ray
            )
        {
            yield return new WaitForSeconds(centerAttackTime);
            var object1 = Instantiate(rayNearEssence, mainObject.position, Quaternion.identity);
            var object2 = Instantiate(rayNearEssence, centerObject.position, Quaternion.identity);
            var scaler = mainObject.localScale;
            var scaler2 = mainObject.localScale;
            object2.transform.localScale = scaler2;
            scaler.x *= -1;
            object1.transform.localScale = scaler;
            var calculations = centerObject.position.x - mainObject.position.x;
            var spawnPosition = centerObject.position;
            var direction = calculations / Mathf.Abs(calculations);
            var positionTo = new Vector3(mainObject.position.x + direction, mainObject.position.y);
            while (spawnPosition != positionTo)
            {
                spawnPosition = new Vector3(
                    spawnPosition.x - direction, 
                    spawnPosition.y, 
                    0);
                Instantiate(ray, spawnPosition, Quaternion.identity);
            }
            yield return new WaitForSeconds(endAttackTime);
            (centerObject.position, mainObject.position) = (mainObject.position, centerObject.position);
            mainObject.localScale = scaler;
            centerObject.localScale = scaler2;
        }
    }
}
