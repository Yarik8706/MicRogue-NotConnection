namespace Enemies
{
    public class MimicChest : TheEnemy
    {
        protected override void Start()
        {
            base.Start();
            animator.Play("Start");
        }
    }
}