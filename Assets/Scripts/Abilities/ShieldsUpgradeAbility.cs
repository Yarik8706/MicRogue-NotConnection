using System;
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

        private GameObject[] newShields;

        public override void Initial(Image button)
        {   
            base.Initial(button);
            newShields = new GameObject[additionalShieldsPositions.Length];
            for (int i = 0; i < additionalShieldsPositions.Length; i++)
            {
                newShields[i] = Instantiate(shield, additionalShieldsPositions[i], Quaternion.identity);
            }
        } 

        public override void DeleteAbility()
        {
            base.DeleteAbility();
            while (newShields.Length != 0)
            {
                Destroy(newShields[0]);
            }
        }
    }
}