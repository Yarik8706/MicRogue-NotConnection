using System;
using System.Collections.Generic;
using System.Linq;
using PlayersScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Abilities
{
    [Serializable]
    [CreateAssetMenu(fileName = "ShieldsUpgradeAbility", menuName = "Ability/ShieldsUpgrade")]
    public class ShieldsUpgradeAbility : CustomAbility
    {
        [SerializeField] private Vector2[] additionalShieldsPositions;
        [SerializeField] private GameObject shield;

        public override void InitialAbility(Player player)
        {
            for (int i = 0; i < additionalShieldsPositions.Length; i++)
            {
                var newShield = Instantiate(shield, player.transform);
                newShield.transform.position =
                    new Vector3(player.transform.position.x + additionalShieldsPositions[i].x,
                        player.transform.position.y + additionalShieldsPositions[i].y, 0);
            }
        } 

        public override void DeleteAbility(Player player)
        {
            base.DeleteAbility(player);
            var actualAdditionalShieldsPositions =
                TheEssence.VariantsPositionsNow(player.transform.position, additionalShieldsPositions).ToList();
            var additionalShields = new List<GameObject>();
            for (int i = 0; i < player.transform.childCount; i++)
            {
                var playerTransformChild = player.transform.GetChild(i);
                if (actualAdditionalShieldsPositions.Contains(playerTransformChild.position))
                {
                    additionalShields.Add(playerTransformChild.gameObject);
                }
            }

            while (additionalShields.Count > 0)
            {
                Destroy(additionalShields[0]);
            }
        }
    }
}