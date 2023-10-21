using System;
using System.Linq;
using MainScripts;
using PlayersScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Abilities
{
    [Serializable]
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability/movement")]
    public class MovementCustomAbility : CustomAbility
    {
        [SerializeField] private Vector2[] positions;

        public override void InitialAbility(Player player)
        {
            var newPositions = new Vector2[player.variantsPositions.Length + positions.Length];
            player.variantsPositions.CopyTo(newPositions, 0);
            positions.CopyTo(newPositions, player.variantsPositions.Length);
            player.variantsPositions = newPositions;
            player.DeleteAllMoveToPlaces();
        }

        public override void DeleteAbility(Player player)
        {
            var positionsList = positions.ToList();
            player.variantsPositions = player.variantsPositions
                .Where(position => !positionsList.Contains(position)).ToArray();
        }
    }
}