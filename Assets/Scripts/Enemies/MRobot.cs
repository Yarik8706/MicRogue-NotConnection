using System.Linq;
using Traps;
using UnityEngine;

namespace Enemies
{
    public class MRobot : TheEnemy
    {
        [SerializeField] private LayerMask trapLayer;

        protected override Vector2 SelectMovePosition(Vector2 playerPosition, Vector2[] theVariantsPositions)
        {
            var safePositions = theVariantsPositions.Where(
                position =>
                {
                    var trap = Ninja.GetPlaceObject(position, trapLayer);
                    if (trap == null) return true;
                    return !trap.GetComponent<TheTrap>().NextStageIsAttack();
                });
            return base.SelectMovePosition(playerPosition, safePositions.ToArray());
        }
    }
}