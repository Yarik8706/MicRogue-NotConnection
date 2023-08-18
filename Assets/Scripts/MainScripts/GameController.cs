using System;
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
        
        internal readonly List<TheEnemy> allEnemies = new();
        internal readonly List<TheTrap> allTraps = new();
        internal float enemyMovementSpeed;
        internal float enemyAnimationSpeed;
        
        public const string EnemyMovementSpeedKeyName = "EnemyMovementSpeed";
        public const string EnemyAnimationSpeedKeyName = "EnemyAnimationSpeed";

        public static GameController instance;

        private void Awake()
        {
            instance = this;
            enemyAnimationSpeed = PlayerPrefsSafe.GetFloat(EnemyAnimationSpeedKeyName, 1);
            enemyMovementSpeed = PlayerPrefsSafe.GetFloat(EnemyMovementSpeedKeyName, .1f);
        }

        public IEnumerator ActiveEnemiesAndTraps()
        {
            ActivateTraps();
            allEnemies.Clear();
            GameplayEventManager.OnGetAllEnemies.Invoke();
            if (allEnemies != null)
            {
                yield return new WaitForSeconds(0.9f);
                yield return ActivateEnemies();
            }
            StartCoroutine(GameManager.instance.TurnStarted());
        }

        private void ActivateTraps()
        {
            allTraps.Clear();
            GameplayEventManager.OnGetAllTraps.Invoke();
            foreach(var trap in allTraps)
            {
                var component = trap.GetComponent<TheTrap>();
                if (!component.isActive) continue;
                component.SetStageAttack(1);
            }
            allTraps.Clear();
        }

        private IEnumerator ActivateEnemies()
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
            allEnemies.Clear();
        }

        private IEnumerator ActiveCertainEnemies(int type)
        {
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

        public static IEnumerator WaitTurnOver(Action endAction)
        {
            yield return new WaitUntil(() => GameManager.player.isTurnOver);
            yield return new WaitUntil(() => !GameManager.player.isTurnOver);
            if (GameManager.player == null) yield break;
            endAction.Invoke();
        }
    }
}