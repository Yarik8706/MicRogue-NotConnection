using System;
using Enemies;
using PlayersScripts;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability/flyingeye")]
    public class ChangingPositionAbility : CustomAbility
    {
        [SerializeField] private float endAttackTime;
        [SerializeField] private float centerAttackTime;
        [SerializeField] private GameObject rayNearEssence;
        [SerializeField] private GameObject ray;
        [SerializeField] private LayerMask objectLayers;

        public override void ActiveAbility(Player player)
        {
            var hit = Physics2D.Raycast(
                player.transform.position,
                player.transform.right,
                30f,
                objectLayers);
            if (hit.collider == null
                || !hit.collider.gameObject.TryGetComponent<TheEssence>(out var enemy)) return;
            player.DeleteAllMoveToPlaces();
            CoroutineController.instance.StartCoroutine(
                CoroutineController.instance.CoroutineWithCallback(
                    FlyingEye.ChangingPositionsOfTwoEntities(
                        player,
                        enemy,
                        endAttackTime,
                        centerAttackTime,
                        rayNearEssence,
                        ray
                    ),
                    player.Active
                ));
        }
    }
}