using System;
using UnityEngine;

namespace Enemies.EnemyObjects
{
    public class Fire : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<IFireAttack>() is {} component)
            {
                component.FireDamage(this);
            }
            else
            {
                if (other.gameObject.GetComponent<TheEssence>() is { } essence)
                {
                   essence.Died(this);  
                }
            }
        }

        public void Died()
        {
            Destroy(gameObject);
        }
    }
}
