using System.Linq;
using Abilities;
using Traps;
using UnityEngine;

namespace Enemies
{
    public class MRobot : TheEnemy, IFireAttack, IColdAttack, IPetrificationAttack
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

        public void FireDamage(GameObject firePrefab)
        {
            Instantiate(firePrefab, transform.position, Quaternion.identity);
        }

        public void FireDamage(MonoBehaviour killer) {}

        public void ColdAttack() {}

        public void Petrification() {}
    }
}