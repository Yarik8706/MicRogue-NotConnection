using UnityEngine;

namespace Enemies.EnemyObjects
{
    public class Fire : MonoBehaviour, ICauseOfDied
    {
        public string[] causeOfDied;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            // if (other.gameObject.GetComponent<IActiveObject>() == null) return;
            if (other.gameObject.GetComponent<IFireAttack>() is {} component)
            {
                component.FireDamage();
            }
            else
            {
                if (other.gameObject.GetComponent<TheEssence>() is { } essence)
                {
                   essence.Died(gameObject);  
                }
            }
        }

        public void Died()
        {
            Destroy(gameObject);
        }

        public string GetDeathText()
        {
            return causeOfDied[Random.Range(0, causeOfDied.Length)];
        }
    }
}
