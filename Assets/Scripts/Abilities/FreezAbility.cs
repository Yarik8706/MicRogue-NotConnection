using System;
using Enemies;
using MainScripts;
using PlayersScripts;
using RoomObjects;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    [CreateAssetMenu(fileName = "FreezAbility", menuName = "Ability/FreezAbility")]
    public class FreezAbility : CustomAbility
    {
        [SerializeField] private GameObject freezingEffect;
        [SerializeField] private FreezingController freezingController;
        
        public override void ActiveAbility(Player player)
        {
            UpdatePlayerTurnAfterUseSpell(player);
            SnowMan.FreezingAllEnemies(freezingController, freezingEffect);
        }
    }
}