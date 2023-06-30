using System;
using Enemies;
using MainScripts;
using RoomObjects;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    [CreateAssetMenu(fileName = "SlimeAbility", menuName = "Ability/SlimeAbility")]
    public class SlimeAbility : CustomAbility
    {
        [SerializeField] private GameObject slimeEffect;
        [SerializeField] private float freezingDistance;
        [SerializeField] private SlimeTrap slimeTrap;
        
        public override void ActiveAbility()
        {
            base.ActiveAbility();
            GameplayEventManager.OnGetAllEnemies.Invoke();
            foreach (var enemy in GameController.instance.allEnemies)
            {
                if(Vector2.Distance(enemy.transform.position, 
                       GameManager.player.transform.position)
                   > freezingDistance
                   ) continue;
                if (enemy is IStuckInSlime stuckInSlime)
                {
                    stuckInSlime.Stuck(slimeTrap);
                }
                else
                {
                    Instantiate(slimeTrap, enemy.transform).Initializate(enemy);
                }

                Instantiate(slimeEffect, enemy.transform.position, Quaternion.identity);
            }
            
        }
    }
}