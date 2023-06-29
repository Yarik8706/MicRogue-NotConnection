using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MainScripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies
{
    public abstract class TheEnemy : TheEssence, ICauseOfDied
    {
        [Header("Enemy Components")]
        public GameObject afterDied;
        public CustomAbility enemyAbility;

        [Header("Enemy Setting")]
        public int enemyType;
        public int enemyCount = 1;
        public string[] causeOfDied;
        
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

        protected virtual bool CheckPlayersShields(Vector2 playerPosition)
        {
            var hit = Physics2D.Linecast(transform.position, playerPosition, GameController.instance.shieldLayer);
            boxCollider2D.enabled = true;
            return hit.collider != null && GameManager.player
                .shieldsControllerUI.RemainingShieldsCount != 0;
        }

        protected virtual IEnumerator AttackPlayer(Vector2 playerPosition)
        {
            boxCollider2D.enabled = false;
            var enemyPosition = transform.position;
            if (!CheckPlayersShields(playerPosition))
            {
                StartCoroutine(Move(playerPosition));
                yield break;
            } 
            var attackVector = ((Vector2)transform.position - playerPosition).normalized;
            var attackPosition = playerPosition + attackVector * attackLong;
            Vector2 endPosition;

            if (((Vector2)transform.position - playerPosition).sqrMagnitude <= 2f)
            {
                endPosition = enemyPosition;
            }
            else
            {
                endPosition = new Vector2((playerPosition.x + enemyPosition.x) / 2, 
                    (playerPosition.y + enemyPosition.y) / 2);
            }
            
            StartCoroutine(SmoothMovement(attackPosition));
            yield return new WaitUntil(() => !isMove);
            GameManager.player.LossOfShieldEvent();
            StartCoroutine(SmoothMovement(endPosition));
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