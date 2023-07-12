using System;
using Enemies;
using MainScripts;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    [CreateAssetMenu(fileName = "NinjaAbility", menuName = "Ability/NinjaAbility")]
    public class NinjaAbility : CustomAbility
    {
        [SerializeField] private Vector2[] ninjaMovePositions;
        [SerializeField] private GameObject ninjaMoveToPlace;
        [SerializeField] private LayerMask blockingLayer;
        
        public override void ActiveAbility()
        {
            base.ActiveAbility();
            var activeNinjaMovePositions = 
                TheEssence.VariantsPositionsNow(GameManager.player.transform.position, ninjaMovePositions);
            for (int i = 0; i < ninjaMovePositions.Length; i++)
            {
                if (Ninja.CheckEmptyPlace(activeNinjaMovePositions[i], blockingLayer))
                {
                    GameManager.player.moveToPlaces.Add(
                        Instantiate(ninjaMoveToPlace, activeNinjaMovePositions[i], Quaternion.identity));
                }
            }
        }
    }
}