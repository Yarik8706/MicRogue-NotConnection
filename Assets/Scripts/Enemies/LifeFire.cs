using RoomObjects;
using UnityEngine;

namespace Enemies
{
    public interface IFireAttack
    {
        public void FireDamage(GameObject fire);

        public void FireDamage();
    }
    
    public class LifeFire : TheEnemy, IColdAttack, IFireAttack
    {
        public LayerMask noFireLayer;
        public Vector2[] firePosition;
        public GameObject fire;

        public override void Died()
        {
            var newVariantPosition = VariantsPositionsNow(firePosition);
            foreach (var position in newVariantPosition)
            {
                boxCollider2D.enabled = false;
                
                var hit = Physics2D.Linecast(transform.position, position, noFireLayer);
                
                boxCollider2D.enabled = true;
                //луч ни с чем не пересёкся
                if (hit.collider == null)
                {
                    Instantiate(fire, position, Quaternion.identity);
                }
                else if(hit.collider.gameObject.GetComponent<BrokenWall>() is {} fireAttack)
                {
                    fireAttack.FireDamage(fire);
                }
            }
            base.Died();
        }

        public void ColdAttack()
        {
            base.Died();
        }

        public void FireDamage(GameObject fire)
        {
            Instantiate(fire, transform.position, Quaternion.identity);
        }
        
        public void FireDamage() {}
    }
}
