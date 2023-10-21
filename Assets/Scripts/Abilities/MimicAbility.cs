using System;
using MainScripts;
using PlayersScripts;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability/mimic")]
    public class MimicAbility : CustomAbility
    {
        [SerializeField] private GameObject updateConsumablesEffect;
        
        public override void ActiveAbility(Player player)
        {
            Instantiate(updateConsumablesEffect, player.transform.position, Quaternion.identity);
            player.ResetConsumables();
        }
    }
}