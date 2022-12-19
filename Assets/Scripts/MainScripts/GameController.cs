using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Canvas;
using Enemies;
using Traps;
using UnityEngine;

namespace MainScripts
{
    public class GameController : MonoBehaviour
    {
        public CanvasInformationAboutObject canvasInformationAboutObject;
        public LayerMask shieldLayer;
        public GameObject roomLight;
        
        [HideInInspector]public GameObject[] allEnemies;
        [HideInInspector]public GameObject[] allEssence;
        [HideInInspector]public bool enemiesActive;
        [HideInInspector]public float enemyMovementSpeed;
        [HideInInspector]public float enemyAnimationSpeed;
        public const string EnemyMovementSpeedKeyName = "EnemyMovementSpeed";
        public const string EnemyAnimationSpeedKeyName = "EnemyAnimationSpeed";

        public static GameController instance;

        private void Awake()
        {
            instance = this;
            enemyAnimationSpeed = PlayerPrefsSafe.GetFloat(EnemyAnimationSpeedKeyName, 1);
            enemyMovementSpeed = PlayerPrefsSafe.GetFloat(EnemyMovementSpeedKeyName, .1f);
        }

        public IEnumerator Active()
        {
            enemiesActive = true;
            //---------------- активируются ловушки----------------
            foreach(var trap in GameObject.FindGameObjectsWithTag("Trap"))
            {
                var component = trap.GetComponent<TheTrap>();
                if (!component.isActive) continue;
                component.SetStageAttack(1);
            }
            yield return new WaitForSeconds(1f);
            //--------------------------------------------------
            allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            StartCoroutine(allEnemies != null ? ActiveEnemies() : GameManager.instance.TurnStarted());
        }

        private IEnumerator ActiveEnemies()
        { // активирует кождого врага по очереди по их типу
            var allEnemiesType = new List<int>();
            allEnemiesType.AddRange(
                from enemy in allEnemies 
                where enemy != null 
                select enemy.GetComponent<TheEnemy>() 
                into enemyClass
                select enemyClass.enemyType);
            if (allEnemiesType.Count != 0)
            {
                var maxType = allEnemiesType.Max();
                var minType = allEnemiesType.Min();
                for (var i = minType; i <= maxType; i++)
                {
                    yield return ActiveCertainEnemies(i);
                }
            }

            StartCoroutine(GameManager.instance.TurnStarted());
        }

        private IEnumerator ActiveCertainEnemies(int type)
        { // просто знай что эта функция активирует определеных врагов 
            var enemies = new List<TheEnemy>();
            enemies.AddRange(
                from enemy in allEnemies 
                where enemy != null 
                select enemy.GetComponent<TheEnemy>() 
                into enemyClass 
                where enemyClass.enemyType == type && enemyClass.isActive
                select enemyClass);
            while (enemies.Count != 0)
            {
                enemies[0].Active();
                yield return new WaitForSeconds(0.1f);
                yield return new WaitUntil(() => enemies[0].isTurnOver);
                enemies.Remove(enemies[0]);
            }
        }
    }
}