using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemies
{
    public class RandomFire : LifeFire
    {
        [SerializeField] private int HowManyFiresAfterDied;
        [Header("Random")]
        public float fireAfterDieChance;
        public override void Died()
        {
            if (Random.value < (fireAfterDieChance / 100f)) //Random.value даёт результат от 0 до 1 в float
            {
                Shuffle(firePosition); // Перемешиваем массив
                for (int i = 0; i < HowManyFiresAfterDied; i++) // Спавним огонь
                {
                    var newVariantPosition = VariantsPositionsNow(firePosition); 
                    boxCollider2D.enabled = false;
                    var hit = Physics2D.Linecast(transform.position, newVariantPosition[i], noFireLayer);
                    boxCollider2D.enabled = true;
                    if (hit.collider == null)
                    {
                        Instantiate(fire, newVariantPosition[i], Quaternion.identity);
                    }
                    else if (hit.collider.gameObject.GetComponent<IFireAttack>() is { } fireAttack)
                    {
                        fireAttack.FireDamage(fire);
                    }
                }
                // строчки с следующей до 36 это метод Died y TheEssence тк это единственная часть скрипта смерти без которой никак
                StartAnimationTrigger(diedAnimation);
                Instantiate(baseAnimationsObj, transform.position, Quaternion.identity)
                    .GetComponent<BaseAnimations>().DiedAnimation();
                TurnOver();
                Destroy(gameObject);
            }
        }
        public void Shuffle(Vector2[] var)
        {
            for (int i = 0; i < var.Length; i++)
            {
                Vector2 temp = var[i];
                int randomIndex = Random.Range(0, var.Length);
                var[i] = var[randomIndex];
                var[randomIndex] = temp;
            }
        }
    }
}
