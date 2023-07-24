using MainScripts;
using RoomObjects;
using UnityEngine;

namespace Enemies
{
    public interface IFireAttack
    {
        public void FireDamage(GameObject firePrefab);

        public void FireDamage();
    }
    
    public class LifeFire : TheEnemy, IColdAttack, IFireAttack, IStuckInSlime
    {
        public LayerMask noFireLayer;
        public Vector2[] firePosition;
        public GameObject fire;

        private BoxCollider2D _boxCollider2D;

        protected override void Start()
        {
            base.Start();
            _boxCollider2D = GetComponent<BoxCollider2D>();
        }

        public override void Died()
        {
            SpawnFire(transform.position, fire, noFireLayer, firePosition, _boxCollider2D);
            base.Died();
        }

        public override void Died(MonoBehaviour killer)
        {
            if (killer is Player)
            {
                CoroutineController.instance.StartCoroutine(GameManager.cameraShake.Shake(0.3f, 0.5f));
            }
            base.Died(killer);
        }

        public static void SpawnFire(Vector3 centralPosition, GameObject fire, 
            LayerMask noFireLayer, Vector2[] firePosition, Collider2D collider2D = null)
        {
            
            var newVariantPosition = 
               VariantsPositionsNow(centralPosition, firePosition);
            if (collider2D != null) collider2D.enabled = false;
            foreach (var position in newVariantPosition)
            {
                
                var hit = Physics2D.Linecast(centralPosition, position, noFireLayer);
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
            if (collider2D != null) collider2D.enabled = true;
        }

        public void ColdAttack()
        {
            base.Died();
        }

        public void FireDamage(GameObject firePrefab)
        {
            Instantiate(firePrefab, transform.position, Quaternion.identity);
        }
        
        public void FireDamage() {}
        
        public void Stuck(SlimeTrap slimeTrap)
        {
            base.Died();
        }
    }
}
