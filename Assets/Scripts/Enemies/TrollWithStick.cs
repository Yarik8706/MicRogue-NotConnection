using System.Collections;
using System.Linq;
using MainScripts;
using UnityEngine;

namespace Enemies
{
    public class TrollWithStick : TheEnemy
    {
        public GameObject enemyShield;
        private bool _startSpawnShield;
        private GameObject _enemyShieldNow;

        protected override void Start()
        {
            base.Start();
            GameplayEventManager.OnNextRoom.AddListener(NextRoom);
        }

        private void NextRoom()
        {
            Destroy(_enemyShieldNow);
        }

        protected override void TurnOver()
        {
            StartCoroutine(InstantiateEnemyShieldAndTurnOver());
        }

        public void StartSpawnShield()
        {
            _startSpawnShield = true;
        }

        private IEnumerator InstantiateEnemyShieldAndTurnOver()
        {
            // троль берет случайного монстра завершившего ход и ставит на нем щит
            var allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            var enemiesNotTurnOver = GameController.instance.allEnemies;
            if(allEnemies == null || enemiesNotTurnOver == null)
            {
                TurnOver();
                yield break;
            }
            var availableTransformsEnemies = (from enemy in allEnemies
                    where enemy != gameObject
                    select enemy.GetComponent<TheEnemy>() 
                    into enemyClass
                    where enemyClass.isActive
                    select enemyClass.transform).ToArray();
            var availablePositionEnemies = availableTransformsEnemies.Select(i => (Vector2)i.position).ToList();
            if (availablePositionEnemies.Count != 0)
            {
                animator.SetTrigger("SpawnEnemyShield");
                yield return new WaitUntil(() => _startSpawnShield);
                _enemyShieldNow = Instantiate(enemyShield,
                    availableTransformsEnemies[availablePositionEnemies.IndexOf(
                            SelectionOfTheNearestPosition(
                                GameManager.player.transform.position, 
                                availablePositionEnemies.ToArray()))]);
            }
            yield return null;
            _startSpawnShield = false;
            base.TurnOver();
        }
    }
}
