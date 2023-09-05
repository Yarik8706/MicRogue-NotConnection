using System;
using Enemies;
using MainScripts;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability/fire")]
    public class FireCustomAbility : CustomAbility
    {
        [SerializeField] private GameObject fire;
        [SerializeField] private LayerMask noFireLayer;
        [SerializeField] private Vector2[] firePositions;

        public override void ActiveAbility()
        { 
            base.ActiveAbility();
            UpdatePlayerTurnAfterUseSpell();
            LifeFire.SpawnFire(GameManager.player.transform.position, fire, noFireLayer, firePositions);
        }
    }
}