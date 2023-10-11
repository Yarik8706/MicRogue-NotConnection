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
        
        protected override void SelectAction(Vector2 nextPosition)
        {
            if (VariantsPositionsNow(firePositions).Any(position => 
                    position == enemyTargetPosition))
            {
                Died();
                return;
            }
            base.SelectAction(nextPosition);
        }

        public override void Died()
        {
            LifeFire.SpawnFire(transform.position, fire, noFireLayer, firePositions);
            base.Died();
        }
    }
}