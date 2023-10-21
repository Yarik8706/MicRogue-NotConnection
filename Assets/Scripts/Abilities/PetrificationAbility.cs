using System;
using System.Collections.Generic;
using Enemies;
using MainScripts;
using PlayersScripts;
using UnityEngine;

namespace Abilities
{
    public interface IPetrificationAttack
    {
        public void Petrification();
    }
    
    [Serializable]
    [CreateAssetMenu(fileName = "PetrificationAbility", menuName = "Ability/PetrificationAbility")]
    public class PetrificationAbility : CustomAbility
    {
        [SerializeField] private GameObject enemyStatue;
        [SerializeField] private Material petrificationMaterial;
        [SerializeField] private int petrificationRange;
        [SerializeField] private GameObject petrificationEffect;
        [SerializeField] private Material greyMaterial;

        public override void ActiveAbility(Player player)
        {
            var targetEnemies = new List<TheEssence>();
            foreach (var enemy in GameplayEventManager.GetAllEnemies())
            {
                if (!(Vector2.Distance(enemy.transform.position,
                        player.transform.position) <= petrificationRange)) continue;
                targetEnemies.Add(enemy);
            }

            CoroutineController.instance.StartCoroutine(Duck.PetrifySomeEssence(enemyStatue,
                petrificationMaterial, petrificationEffect, targetEnemies.ToArray(), greyMaterial));
        }
    }
}