using UnityEngine;

namespace Enemies
{
    public class Orge : TheEnemy
    {
        public Vector2[] variantsPositions1;
        public Vector2[] variantsPositions2;

        protected override void Start()
        {
            base.Start();
            variantsPositions = variantsPositions1;
        }

        public override void TurnOver()
        {
            base.TurnOver();
            if(essenceEffect == TheEssenceEffect.Freezing) return;
            if (variantsPositions == variantsPositions2)
            {
                animator.Play("Idle1");
                moveAnimation = new AnimationType("Run1", moveAnimation.speed);
                variantsPositions = variantsPositions1;
            }
            else
            {
                animator.Play("Idle2");
                moveAnimation = new AnimationType("Run2", moveAnimation.speed);
                variantsPositions = variantsPositions2;
            }
        }
    }
}
