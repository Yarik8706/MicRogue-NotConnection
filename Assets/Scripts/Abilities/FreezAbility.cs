using System;
using Enemies;
using MainScripts;
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
        
        public override void ActiveAbility()
        {
            base.ActiveAbility();
            SnowMan.FreezingAllEnemies(freezingController, freezingEffect);
        }
    }
}