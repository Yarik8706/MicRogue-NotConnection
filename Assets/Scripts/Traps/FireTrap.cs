using System.Collections;
using Enemies;

namespace Traps
{
    public class FireTrap : TheTrap
    {
        public ExplosionOfLight explosionLight;
        protected override IEnumerator Attack()
        {
            animator.SetTrigger(attack);
            explosionLight.StartExplosionLight();
            while (attackObjects.Count != 0)
            {
                if (attackObjects[0].GetComponent<IFireAttack>() is { } component)
                {
                    component.FireDamage();
                }
                else if (attackObjects[0].GetComponent<TheEssence>() is {} essence)
                {
                    essence.Died(this);
                }
                if(attackObjects.Count != 0) attackObjects.RemoveAt(0);
            }
            yield return base.Attack();
        }
    }
}
