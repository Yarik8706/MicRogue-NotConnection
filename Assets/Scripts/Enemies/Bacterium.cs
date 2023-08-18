using System.Collections;
using System.Collections.Generic;
using MainScripts;
using UnityEngine;

namespace Enemies
{
    public class Bacterium : TheEnemy
    {
        private Vector2 _oldPlayerPosition;
        
        protected override void Start()
        {
            _oldPlayerPosition = GameManager.player.transform.position;
            base.Start();
        }

        public override void TurnOver()
        {
            _oldPlayerPosition = GameManager.player.transform.position;
            base.TurnOver();
        }

        protected override Vector2 SelectMovePosition(Vector2 playerPosition, Vector2[] theVariantsPositions)
        {
            if (base.SelectMovePosition(playerPosition, theVariantsPositions) == playerPosition)
            {
                return playerPosition;
            }
            var nextVector2 = playerPosition - _oldPlayerPosition;
            var nextPositions = new List<Vector2> {new(-nextVector2.x, nextVector2.y)};
            if (Mathf.Abs(nextVector2.x) - 2 == 0)
            {
                nextPositions.Add(new(-nextVector2.x/2, nextVector2.y));
            }
            else if (Mathf.Abs(nextVector2.y) - 2 == 0)
            {
                nextPositions.Add(new(-nextVector2.x, nextVector2.y/2));
            }
            
            var resultPositions = MoveCalculation(VariantsPositionsNow(nextPositions.ToArray()));
            return resultPositions.Length == 0 ? transform.position : resultPositions[0];
        }
        
        protected override IEnumerator AttackPlayer(Vector2 playerPosition)
        {
            yield return Move(playerPosition);
        }
    }
}