using System;
using Enemies;
using PlayersScripts;
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
        
        public override void ActiveAbility(Player player)
        {
            var activeNinjaMovePositions = 
                TheEssence.VariantsPositionsNow(player.transform.position, ninjaMovePositions);
            for (int i = 0; i < ninjaMovePositions.Length; i++)
            {
                if (Ninja.CheckEmptyPlace(activeNinjaMovePositions[i], blockingLayer))
                {
                    var moveToPlace =
                        Instantiate(ninjaMoveToPlace, activeNinjaMovePositions[i], Quaternion.identity);
                    moveToPlace.GetComponent<MoveToPlace>().player = player;
                    player.moveToPlaces.Add(moveToPlace);
                }
            }
        }
    }
}