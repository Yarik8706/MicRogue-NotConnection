using System;
using MainScripts;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability/mimic")]
    public class MimicAbility : CustomAbility
    {
        [SerializeField] private GameObject updateConsumablesEffect;
        
        public override void ActiveAbility()
        {
            base.ActiveAbility();
            Instantiate(updateConsumablesEffect, GameManager.player.transform.position, Quaternion.identity);
            GameManager.player.ResetConsumables();
        }
    }
}