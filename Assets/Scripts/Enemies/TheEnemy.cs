using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MainScripts;
using UnityEngine;
using Random = UnityEngine.Random;


public struct MyStr
{
    public string name;
    public int age;
}

namespace Enemies
{
    public abstract class TheEnemy : TheEssence, ICauseOfDied
    {
        [Header("Enemy Components")]
        public GameObject afterDied;
        
        [Header("Enemy Setting")]
        public int enemyType;
        public int enemyCount = 1;
        public string[] causeOfDied; // появляется в сообщении о смерти
        
        protected BaseAnimations baseAnimations;
        internal SpriteRenderer spriteRenderer;
        private float attackLong = 0.6f;
        
        protected override void Start()
        {
            base.Start();
            spriteRenderer = GetComponent<SpriteRenderer>();
            GameplayEventManager.OnNextRoom.AddListener(NextRoomEvent);
            SetAnimationMoveSpeed(
                GameController.instance.enemyAnimationSpeed, 
                GameController.instance.enemyMovementSpeed
                );
        }

        public override void Active()
        {
            base.Active();
            if (GameManager.player == null)
            {
                TurnOver();
                return;
            }
            SelectAction(SelectMovePosition(
                    GameManager.player.transform.position,
                    MoveCalculation(
                        VariantsPositionsNow(variantsPositions))), 
                GameManager.player.transform.position);
        }

        public static Vector2 SelectionOfTheNearestPosition(
            Vector2 comparablePosition, 
            Vector2[] positions, 
            Vector2 defaultPosition)
        {
            var distances = new List<float>(positions.Length);
            distances.AddRange(positions.Select(
                nowVariantPosition => Vector2.Distance(nowVariantPosition, comparablePosition))
            );
            return positions.Length != 0 ? positions[distances.IndexOf(distances.Min())] : defaultPosition;
        }
        
        public static Vector2 SelectionOfTheNearestPosition(
            Vector2 comparablePosition, 
            Vector2[] positions)
        {
            var distances = new List<float>(positions.Length);
            distances.AddRange(positions.Select(
                nowVariantPosition => Vector2.Distance(nowVariantPosition, comparablePosition))
            );

            return positions[distances.IndexOf(distances.Min())];
        } 

        protected virtual Vector2 SelectMovePosition(Vector2 playerPosition, Vector2[] theVariantsPositions)
        {
            return SelectionOfTheNearestPosition(playerPosition, theVariantsPositions, transform.position);
        }

        protected virtual void SelectAction(Vector2 nextPosition, Vector2 playerPosition)
        {
            if (nextPosition == (Vector2)transform.position)
            {
                TurnOver();
                return;
            }
            if(nextPosition.x < transform.position.x && turnedRight 
               || nextPosition.x > transform.position.x && !turnedRight)
            {
                Flip();
            }
            StartCoroutine(nextPosition == playerPosition ? AttackPlayer(nextPosition) : Move(nextPosition));
        }

        protected virtual IEnumerator AttackPlayer(Vector2 playerPosition)
        {
            boxCollider2D.enabled = false;
            var position = transform.position;
            var hit = Physics2D.Linecast(position, playerPosition, GameController.instance.shieldLayer);
            boxCollider2D.enabled = true;
            if (hit.collider == null || GameManager.player.shieldsCount == 0)
            {
                StartCoroutine(Move(playerPosition));
                yield break;
            }
            if (hit.collider.gameObject.layer != 7)
            {
                yield break;
            }
            Vector2 attackPosition;
            Vector2 nextPosition;
            if ((new Vector2(position.x, position.y) - playerPosition).sqrMagnitude <= 2f)
            {
                nextPosition = position;
                attackPosition = new Vector2((playerPosition.x + position.x)/2, 
                    (playerPosition.y + position.y) * 0.5f);
            }
            else
            {
                nextPosition = new Vector2((playerPosition.x + position.x) / 2, 
                    (playerPosition.y + position.y) / 2);
                if (Math.Abs(playerPosition.y - position.y) > 0)
                {
                    attackPosition = new Vector2(playerPosition.x, 
                        (playerPosition.y + position.y) * attackLong);
                }
                else
                {
                    if (((playerPosition.x < position.x && 0 < position.x)
                         || (playerPosition.x > position.x && playerPosition.x < 0))
                        && playerPosition.x + position.x != 0)
                    {
                        attackPosition = new Vector2((playerPosition.x + position.x) * (1 - attackLong),
                            playerPosition.y);
                    }
                    else
                        attackPosition = playerPosition.x switch
                        {
                            < 0 when position.x > 0 => new Vector2(-attackLong, playerPosition.y),
                            > 0 when position.x < 0 => new Vector2(attackLong, playerPosition.y),
                            _ => new Vector2((playerPosition.x + position.x) * attackLong, playerPosition.y)
                        };
                }
            }
            StartCoroutine(SmoothMovement(attackPosition));
            yield return new WaitUntil(() => !isMove);
            GameManager.player.LossOfShieldEvent();
            StartCoroutine(SmoothMovement(nextPosition));
            yield return new WaitUntil(() => !isMove);
            yield return new WaitForSeconds(0.5f);
            TurnOver();
        }

        protected virtual IEnumerator OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) yield break;
            if (!isMove && GameManager.player.isMove)
            {
                yield return new WaitUntil(() => GameManager.player.isMove == false);
                Died(GameManager.player);
            }
            else
            {
                yield return new WaitUntil(() => isMove == false);
                if (transform.position == GameManager.player.transform.position)
                {
                    GameManager.player.Died(causeOfDied[Random.Range(0, causeOfDied.Length)]);
                }
            }
        }

        protected virtual void NextRoomEvent()
        {
            Destroy(gameObject);
        }
        
        private void OnDestroy()
        {
            if (baseAnimations != null)
            {
                Destroy(baseAnimations.gameObject);
            }
        }

        public override void Died()
        {
            if (afterDied != null)
            {
                Instantiate(afterDied, transform.position, Quaternion.identity);
            }
            base.Died();
        }

        public string GetDeathText()
        {
            return causeOfDied[Random.Range(0, causeOfDied.Length)];
        }
    }
}