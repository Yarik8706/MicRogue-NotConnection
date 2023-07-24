using RoomObjects;
using UnityEngine;

namespace Enemies
{
    public class GreyTroll : Orge
    {
        [SerializeField] private GameObject turnOverEffect;
        
        public override void TurnOver()
        {
            base.TurnOver();
            Instantiate(turnOverEffect, transform.position, Quaternion.identity)
                .GetComponent<FreezingController>().animator.SetTrigger("Frosbite");;
        }
    }
}