using System;
using MainScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Abilities
{
    [Serializable]
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability/movement")]
    public class MovementCustomAbility : CustomAbility
    {
        [SerializeField] private Vector2[] positions;

        private Vector2[] oldPositions;

        public override void Initial(Image button)
        {   
            base.Initial(button);
            oldPositions = GameManager.player.variantsPositions;
            var newPositions = new Vector2[GameManager.player.variantsPositions.Length + positions.Length];
            GameManager.player.variantsPositions.CopyTo(newPositions, 0);
            positions.CopyTo(newPositions, GameManager.player.variantsPositions.Length);
            GameManager.player.variantsPositions = newPositions;
            GameManager.player.DeleteAllMoveToPlaces();
            GameManager.player.Active();
        }

        public override void DeleteAbility()
        {
            base.DeleteAbility();
            GameManager.player.variantsPositions = oldPositions;
        }
    }
}