using System;
using Enemies;
using MainScripts;
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

        public override void ActiveAbility()
        {
            base.ActiveAbility();
            var hit = Physics2D.Raycast(
                GameManager.player.transform.position,
                GameManager.player.transform.right,
                30f,
                objectLayers);
            if (hit.collider == null
                || !hit.collider.gameObject.TryGetComponent<TheEnemy>(out var enemy)) return;
            Debug.Log(hit.collider.gameObject);
            GameManager.player.DeleteAllMoveToPlaces();
            CoroutineController.instance.StartCoroutine(
                CoroutineController.instance.CoroutineWithCallback(
                    FlyingEye.ChangingPositionsOfTwoEntities(
                        GameManager.player.transform,
                        enemy.transform,
                        endAttackTime,
                        centerAttackTime,
                        rayNearEssence,
                        ray
                    ),
                    () => GameManager.player.Active()
                ));
        }
    }
}