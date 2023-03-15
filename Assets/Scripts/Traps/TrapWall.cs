using System.Collections;
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

        protected void Start()
        {
            _shadowCaster2D = GetComponent<ShadowCaster2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animations1 = (string[]) stagesAttack.Clone();
            if (isSecondPhase)
            {
                gameObject.layer = 6;
                stagesAttack = (string[]) animations2.Clone();
                SetStageAttack(0);
            }
        }

        protected override IEnumerator Attack()
        {
            animator.SetTrigger(attack);
            if (attackObjects.Count != 0)
            {
               if (attackObjects[0].GetComponent<ICompressionAttack>() is {} component)
               {
                   component.CompressionDamage();
               }
               else if (attackObjects[0].GetComponent<TheEssence>() is {} essence)
               {
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
            SetStageAttack(0);
            _shadowCaster2D.enabled = false;
        }

        public void SetFloorState()
        {
            stageNow = 0;
            isSecondPhase = false;
            gameObject.layer = 0;
            _spriteRenderer.sortingLayerName = "Default";
            stagesAttack = (string[]) _animations1.Clone();
            StartCoroutine(EndAnimation());
        }

        public override void SetStageAttack(int i)
        {
            if(stageNow + i >= stagesAttack.Length)
            {
                if (isSecondPhase)
                {
                    SetFloorState();
                }
                else
                {
                    StartCoroutine(Attack());
                    _spriteRenderer.sortingLayerName = "Decoration";
                    gameObject.layer = 6;
                    isSecondPhase = true;
                    stagesAttack = (string[]) animations2.Clone();
                }
            }
            else {
                stageNow += i;
                animator.SetTrigger(stagesAttack[stageNow]); 
            }
        }
    }
}
