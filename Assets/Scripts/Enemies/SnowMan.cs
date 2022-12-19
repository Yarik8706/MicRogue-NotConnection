using System.Collections;
using System.Collections.Generic;
using MainScripts;
using Other;
using UnityEngine;

namespace Enemies
{
    public interface IColdAttack
    {
        public void ColdAttack();
    }
    
    public class SnowMan : TheEnemy, IColdAttack, IFireAttack
    {
        
        private bool _isColdEventStart;
        private int _timeColdEvent;

        public override void Active()
        {
            if (_isColdEventStart)
            {
                _timeColdEvent++;
                if (_timeColdEvent == 3)
                {
                    isActive = false;
                }
                TurnOver();
                return;
            }
            base.Active();
        }

        private IEnumerator StartEnemyCold(GameObject enemy)
        {
            var enemyClass = enemy.GetComponent<TheEnemy>();
            var animationObj = Instantiate(baseAnimationsObj, enemy.transform.position, Quaternion.identity);
            var animationObjClass = animationObj.GetComponent<BaseAnimations>();
            animationObj.transform.SetParent(enemy.transform);
            enemyClass.isActive = false;
            animationObjClass.isDied = false;
            animationObjClass.FreezingAnimation();
            animationObj.transform.SetParent(enemy.transform);
            var baseAnimatorSpeed = enemyClass.animator.speed;
            var baseColorSprite = enemyClass.spriteRenderer.color;
            enemyClass.spriteRenderer.color = Color.cyan;
            enemyClass.animator.speed = 0;
            yield return new WaitUntil(() =>
            {
                if (_timeColdEvent == 3) enemyClass.isActive = true;
                return enemyClass == null || enemyClass.isMove;
            });
            if(enemyClass == null) yield break;
            enemyClass.animator.speed = baseAnimatorSpeed;
            animationObjClass.isDied = true;
            enemyClass.spriteRenderer.color = baseColorSprite;
            animationObjClass.FrostbiteAnimation();
        }

        public override void Died()
        {
            boxCollider2D.enabled = false;
            spriteRenderer.enabled = false;
            var allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in allEnemies)
            {
                var enemyClass = enemy.GetComponent<TheEnemy>();
                if (enemy == gameObject || !enemyClass.isActive)
                {
                    continue;
                }
                if (enemy.GetComponent<IColdAttack>() is {} component)
                {
                    component.ColdAttack();
                    continue;
                }
                StartCoroutine(StartEnemyCold(enemy));
            }
            _isColdEventStart = true;
            Instantiate(afterDied, transform.position, Quaternion.identity);
            Instantiate(baseAnimationsObj, transform.position, Quaternion.identity)
                .GetComponent<BaseAnimations>().DiedAnimation();
            StartCoroutine(ColdBlackount());
        }

        private IEnumerator ColdBlackount()
        {
            OtherGameobjectController.instance.coldBlackount.SetActive(true);
            yield return new WaitForSeconds(0.7f);
            OtherGameobjectController.instance.coldBlackount.SetActive(false);
            TurnOver();
        }

        public void ColdAttack(){}

        public void FireDamage(GameObject fire)
        {
            Instantiate(fire, transform.position, Quaternion.identity);
            base.Died();
        }

        public void FireDamage()
        {
            base.Died();
        }
    }
}
