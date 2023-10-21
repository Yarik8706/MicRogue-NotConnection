using Canvas;
using MainScripts;
using RoomObjects;
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

        public override void Died(MonoBehaviour killer)
        {
            if (killer is TheEssence essence)
            {
                Instantiate(freezingEffect, essence.transform.position, Quaternion.identity);
                FreezingEssence(essence, freezingController);
            }
            base.Died(killer);
        }

        public static void FreezingAllEnemies(FreezingController freezingController, 
            GameObject freezingEffect, TheEnemy snowMan = null)
        {
            foreach (var enemy in GameplayEventManager.GetAllEnemies())
            {
                if (enemy == null || enemy == snowMan || !enemy.isActive || enemy.essenceEffect != TheEssenceEffect.None)
                {
                    continue;
                }
                FreezingEssence(enemy, freezingController);
                Instantiate(freezingEffect, enemy.transform.position, Quaternion.identity);
            }
            ColdBlackoutControl.instance.ActivateColdBlackout();
        }

        private static void FreezingEssence(TheEssence essence, FreezingController freezingController)
        {
            if (essence is IColdAttack component)
            {
                component.ColdAttack();
            }
            else
            {
                Instantiate(freezingController, essence.transform).Initializate(essence);
            }
        }

        public void ColdAttack(){}

        public void FireDamage(GameObject firePrefab)
        {
            Instantiate(firePrefab, transform.position, Quaternion.identity);
            base.Died();
        }

        public void FireDamage(MonoBehaviour killer)
        {
            base.Died();
        }
    }
}
