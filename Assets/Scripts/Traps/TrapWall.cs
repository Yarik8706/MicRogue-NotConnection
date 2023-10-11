using System;
using System.Collections;
using Enemies;
using MainScripts;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Traps
{
    public interface ICompressionAttack
    {
        public void CompressionDamage();
    }
    
    public class TrapWall : TheTrap
    {
        public string[] animations2;
        public string endAnimation2;
        public bool isSecondPhase;

        private string[] _animations1;
        private ShadowCaster2D _shadowCaster2D;
        private SpriteRenderer _spriteRenderer;

        protected override void Awake()
        {
            base.Awake();
            _shadowCaster2D = GetComponent<ShadowCaster2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animations1 = stagesAttack;
        }

        private void Start()
        {
            if (isSecondPhase)
            {
                SetBlockState();
            } 
            else 
            {
                SetFloorState();
            }
            SetStageAttack(0);
        }

        public override bool NextStageIsAttack()
        {
            return base.NextStageIsAttack() && !isSecondPhase;
        }

        protected override IEnumerator Attack()
        {
            if (attackObjects.Count != 0 && attackObjects[0].GetComponent<Dragon>() is {})
            {
                SetFloorState();
                yield break;
            }
            animator.SetTrigger(attack);
            if (attackObjects.Count != 0)
            {
               if (attackObjects[0].GetComponent<ICompressionAttack>() is {} component)
               {
                   component.CompressionDamage();
               }
               else if (attackObjects[0].GetComponent<TheEssence>() is {} essence)
               {
                   // if (essence is Player)
                   // {
                   //     yield return new WaitUntil(() => GameManager.player.isTurnOver);
                   //     if (GameManager.player.transform.position != transform.position) yield break;
                   //     yield return null;
                   //     yield return new WaitUntil(() => !GameManager.player.isTurnOver);
                   //     if (GameManager.player == null) yield break;
                   //     if (GameManager.player.transform.position == transform.position)
                   //     {
                   //         
                   //         GameManager.instance.backroomsController.StartBackrooms();
                   //     }
                   //     yield break;
                   // }
                   essence.Died(this);
               }
            }
            yield return base.Attack();
            _shadowCaster2D.enabled = true;
        }

        private IEnumerator EndAnimation()
        {
            animator.SetTrigger(endAnimation2);
            yield return new WaitForSeconds(waitTime);
            stageNow = 0;
            SetStageAttack(0);
            _shadowCaster2D.enabled = false;
        }

        public void SetFloorState()
        {
            isSecondPhase = false;
            gameObject.layer = LayerMask.NameToLayer("Trap");
            _spriteRenderer.sortingLayerName = "Default";
            stagesAttack = _animations1;
        }

        private void SetBlockState()
        {
            _spriteRenderer.sortingLayerName = "Decoration";
            gameObject.layer = LayerMask.NameToLayer("Blocking");
            isSecondPhase = true;
            stagesAttack = animations2;
        }

        public override void SetStageAttack(int i)
        {
            if(stageNow + i >= stagesAttack.Length)
            {
                if (isSecondPhase)
                {
                    SetFloorState();
                    StartCoroutine(EndAnimation());
                }
                else
                {
                    SetBlockState();
                    StartCoroutine(Attack());
                }
            }
            else {
                stageNow += i;
                animator.SetTrigger(stagesAttack[stageNow]); 
            }
        }
    }
}
