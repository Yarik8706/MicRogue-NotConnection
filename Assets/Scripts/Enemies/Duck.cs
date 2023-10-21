using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Abilities;
using MainScripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class Duck : TheEnemy
    {
        [SerializeField] private GameObject enemyStatue;
        [SerializeField] private Material petrificationMaterial;
        [SerializeField] private GameObject petrificationEffect;
        [SerializeField] private Material greyMaterial;
        [SerializeField] private GameObject baseAnimationsObj;
        
        private bool _isCenterAttack;
        private bool _isEndAttack;
        private const string AttackAnimationName = "DuckAttack";
        private const string RayCloseAnimationName = "DuckRayClose";
        private const string RayAnimationName = "DuckRay";

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

        protected override void SelectAction(Vector2 nextPosition)
        {
            if(nextPosition.x < transform.position.x && turnedRight || nextPosition.x > transform.position.x && !turnedRight)
            {
                Flip();
            }
            if (Mathf.Abs(transform.position.y - enemyTarget.transform.position.y) < 0.2f)
            {
                boxCollider2D.enabled = false;
                var hit = Physics2D.Linecast(transform.position, 
                    enemyTarget.transform.position, blockingLayer);
                boxCollider2D.enabled = true;
                if (hit.collider == null)
                {
                    if(enemyTarget.transform.position.x - transform.position.x > 0 && !turnedRight 
                       || enemyTarget.transform.position.x - transform.position.x < 0 && turnedRight)
                    {
                        Flip();
                    }
                    StartCoroutine(AttackPlayer(enemyTarget));
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

        protected override IEnumerator AttackPlayer(TheEssence essence)
        {
            var playerPosition = essence.transform.position;
            animator.Play(AttackAnimationName);
            yield return new WaitUntil(() => _isCenterAttack);
            var object1 = Instantiate(baseAnimationsObj, transform.position, Quaternion.identity);
            var object2 = Instantiate(baseAnimationsObj, playerPosition, Quaternion.identity);
            var scaler = transform.localScale;
            object1.transform.localScale = scaler;
            scaler.x *= -1;
            object2.transform.localScale = scaler;
            object1.GetComponent<Animator>().Play(RayCloseAnimationName);
            object2.GetComponent<Animator>().Play(RayCloseAnimationName);
            yield return new WaitForSeconds(0.3f);
            var calculations = playerPosition.x - transform.position.x;
            var spawnPosition = (Vector3)playerPosition;
            var direction = calculations / Mathf.Abs(calculations);
            var positionTo = new Vector3(transform.position.x + direction, transform.position.y);
            while (spawnPosition != positionTo)
            {
                spawnPosition = new Vector3(
                    spawnPosition.x - direction, 
                    spawnPosition.y, 
                    0);
                Instantiate(baseAnimationsObj, spawnPosition, Quaternion.identity)
                    .GetComponent<Animator>().Play(RayAnimationName);
            }
            yield return new WaitUntil(() => _isEndAttack);
            if (CanKillEssence(essence, essence.transform.position - Vector3.down * 0.1f))
            {
                yield return PetrifyOneEssence(enemyStatue,
                                petrificationMaterial, petrificationEffect, essence, greyMaterial);
            }
            TurnOver();
        }

        public static IEnumerator PetrifySomeEssence(GameObject statuePrefab, 
            Material petrificationMaterial, GameObject petrificationEffect, 
            TheEssence[] essences, Material greyMaterial)
        {
            petrificationMaterial.SetFloat(Shader.PropertyToID("_Fade"), 0);
            var statues = new List<SpriteRenderer>(essences.Length);
            foreach (var essence in essences)
            {
                Instantiate(petrificationEffect, essence.transform.position, Quaternion.identity);
                if (essence is IPetrificationAttack petrification)
                {
                    petrification.Petrification();
                    yield break;
                }
                var spriteRenderer = Instantiate(statuePrefab,
                    essence.transform.position, Quaternion.identity).GetComponent<SpriteRenderer>();
                
                spriteRenderer.sprite = essence.spriteRenderer.sprite;
                spriteRenderer.transform.localScale = essence.transform.localScale;
                statues.Add(spriteRenderer);
                essence.Died(null);
            }
            var number = 0f;
            yield return new WaitForSeconds(0.25f);
            while (number < 1) 
            {
                petrificationMaterial.SetFloat("_Fade", number);
                number += Time.deltaTime / 3;
                yield return null;
            }

            foreach (var statue in statues)
            {
                if(statue != null)
                    statue.material = greyMaterial; 
            }
        }

        public static IEnumerator PetrifyOneEssence(GameObject statuePrefab, 
            Material petrificationMaterial, GameObject petrificationEffect, 
            TheEssence essence, Material greyMaterial)
        {
            yield return PetrifySomeEssence(statuePrefab, petrificationMaterial, petrificationEffect,
                new TheEssence[] {essence}, greyMaterial);
        }

        public void CenterAttack()
        {
            _isCenterAttack = true;
        }

        public void EndAttack()
        {
            _isEndAttack = true;
        }
    }
}
