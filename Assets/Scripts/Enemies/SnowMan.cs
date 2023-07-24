using System.Collections;
using System.Collections.Generic;
using MainScripts;
using Other;
using RoomObjects;
using Traps;
using UnityEngine;

namespace Enemies
{
    public interface IColdAttack
    {
        public void ColdAttack();
    }
    
    public class SnowMan : TheEnemy, IColdAttack, IFireAttack
    {
        [SerializeField] private FreezingController freezingController;
        [SerializeField] private GameObject freezingEffect;

        public override void Died()
        {
            FreezingAllEnemies(freezingController, freezingEffect, this);
            base.Died();
        }

        public static void FreezingAllEnemies(FreezingController freezingController, 
            GameObject freezingEffect, TheEnemy snowMan = null)
        {
            GameplayEventManager.OnGetAllEnemies.Invoke();
            foreach (var enemy in GameController.instance.allEnemies)
            {
                if (enemy == snowMan || !enemy.isActive || enemy.essenceEffect != TheEssenceEffect.None)
                {
                    continue;
                }
                if (enemy is IColdAttack component)
                {
                    component.ColdAttack();
                }
                else
                {
                    Instantiate(freezingController, enemy.transform).Initializate(enemy);
                }
                Instantiate(freezingEffect, enemy.transform.position, Quaternion.identity);
            }
            GameManager.instance.ActivateColdBlackout();
        }

        public void ColdAttack(){}

        public void FireDamage(GameObject firePrefab)
        {
            Instantiate(firePrefab, transform.position, Quaternion.identity);
            base.Died();
        }

        public void FireDamage()
        {
            base.Died();
        }
    }
}
