using System.Collections.Generic;
using System.Linq;
using Canvas;
using MainScripts;
using PlayersScripts;
using Unity.Mathematics;
using UnityEngine;

namespace Enemies
{
    public abstract class TheEnemy : TheEssence
    {
        [Header("Enemy")] 
        public CustomAbility enemyAbility;
        public int enemyType;
        public int enemyCount = 1;
        
        [SerializeField] private GameObject afterDied;
        
        protected TheEssence enemyTarget;
        protected Vector2 enemyTargetPosition;

        protected override void Start()
        {
            base.Start();
            GameplayEventManager.OnNextRoom.AddListener(NextRoomEvent);
            GameplayEventManager.OnGetAllEnemies.AddListener(AddYourselfToEnemyList);
        }

        private void AddYourselfToEnemyList()
        {
            GameController.instance.allEnemies.Add(this);
        }

        public override void Active()
        {
            base.Active();
            if (GameManager.enemyTargets.Count == 0)
            {
                TurnOver();
                return;
            }
            enemyTarget = GetNearestEnemyTarget(transform.position);
            enemyTargetPosition = (Vector2) enemyTarget.transform.position;
            SelectAction(SelectMovePosition(enemyTarget.transform.position,
                    MoveCalculation(VariantsPositionsNow(variantsPositions))));
        }

        public static TheEssence GetNearestEnemyTarget(Vector3 enemyPosition)
        {
            if (GameManager.enemyTargets.Count == 1)
            {
                return GameManager.enemyTargets[0];
            }
            List<float> distance = new List<float>(GameManager.enemyTargets.Count);
            for (int i = 0; i < GameManager.enemyTargets.Count; i++)
            {
                distance.Add(Vector2.Distance(
                    GameManager.enemyTargets[i].transform.position, enemyPosition));
            }

            return GameManager.enemyTargets[distance.IndexOf(distance.Min())];
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

        protected virtual void SelectAction(Vector2 nextPosition)
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
            StartCoroutine(nextPosition == (Vector2)enemyTarget.transform.position 
                ? AttackPlayer(enemyTarget) : Move(nextPosition));
        }

        protected virtual void NextRoomEvent()
        {
            Destroy(gameObject);
        }

        public override void Died(MonoBehaviour killer)
        {
            if (killer is Player && diedMusic != null || killer is StealAbilityController)
            {
                DiedWithMusic();
            }
            base.Died(killer);
        }

        public override void Died()
        {
            if (afterDied != null)
            {
                Instantiate(afterDied, transform.position, Quaternion.identity);
            }
            base.Died();
        }
    }
}