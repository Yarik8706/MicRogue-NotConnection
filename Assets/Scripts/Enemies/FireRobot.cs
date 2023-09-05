using System.Linq;
using MainScripts;
using UnityEngine;

namespace Enemies
{
    public class FireRobot : MRobot
    {
        [SerializeField] private GameObject fire;
        [SerializeField] private LayerMask noFireLayer;
        [SerializeField] private Vector2[] firePositions;
        
        protected override void SelectAction(Vector2 nextPosition, Vector2 playerPosition)
        {
            if (VariantsPositionsNow(firePositions).Any(position => position == playerPosition))
            {
                Died();
                return;
            }
            base.SelectAction(nextPosition, playerPosition);
        }

        public override void Died()
        {
            LifeFire.SpawnFire(transform.position, fire, noFireLayer, firePositions);
            base.Died();
        }
    }
}