using System.Collections;

namespace Traps
{
    public interface IThrustAttack
    {
        public void ThrustAttack();
    }

    public class FloorTrap : TheTrap
    {
        protected override IEnumerator Attack()
        {
            animator.SetTrigger(attack);
            if (attackObjects.Count != 0)
            {
                if (attackObjects[0].GetComponent<IThrustAttack>() is { } component)
                {
                    component.ThrustAttack();
                }
                else if (attackObjects[0].GetComponent<TheEssence>() is { } essence)
                {
                    essence.Died(this);
                }
            }

            yield return base.Attack();
        }
    }
}
