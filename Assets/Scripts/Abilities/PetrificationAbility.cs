using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Enemies;
using MainScripts;
using UnityEngine;

namespace Abilities
{
    public interface IPetrificationAttack
    {
        public void Petrification();
    }
    
    [Serializable]
    [CreateAssetMenu(fileName = "PetrificationAbility", menuName = "Ability/PetrificationAbility")]
    public class PetrificationAbility : CustomAbility
    {
        [SerializeField] private GameObject enemyStatue;
        [SerializeField] private Material petrificationMaterial;
        [SerializeField] private int petrificationRange;
        [SerializeField] private GameObject petrificationEffect;

        public override void ActiveAbility()
        {
            base.ActiveAbility();
            GameController.instance.allEnemies.Clear();
            GameplayEventManager.OnGetAllEnemies.Invoke();
            petrificationMaterial.SetFloat(Shader.PropertyToID("_Fade"), 0);
            foreach (var enemy in GameController.instance.allEnemies)
            {
                if (!(Vector2.Distance(enemy.transform.position,
                        GameManager.player.transform.position) <= petrificationRange)) continue;
                Instantiate(petrificationEffect, enemy.transform.position, Quaternion.identity);
                if (enemy is IPetrificationAttack petrification)
                {
                    petrification.Petrification();
                    continue;
                }

                var spriteRenderer = Instantiate(enemyStatue,
                    enemy.transform.position, enemy.transform.rotation).GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = enemy.spriteRenderer.sprite;
                enemy.Died();
            }

            CoroutineController.instance.StartCoroutine(PetrificationCoroutine());
        }

        private IEnumerator PetrificationCoroutine()
        {
            var number = 0f;
            yield return new WaitForSeconds(0.25f);
            while (number < 1) 
            {
                petrificationMaterial.SetFloat("_Fade", number);
                number += Time.deltaTime / 3;
                yield return null;
            }
        }
    }
}