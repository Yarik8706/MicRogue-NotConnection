using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MainScripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Traps
{
    public abstract class TheTrap : MonoBehaviour, IActiveObject
    {
        [SerializeField] protected float waitTime;
        [SerializeField] protected int stageNow;
        [SerializeField] protected string[] stagesAttack;
        [SerializeField] protected string attack;
        protected Animator animator;
        protected List<GameObject> attackObjects;
        public bool isActive = true;

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
            attackObjects = new List<GameObject>();
            SetStageAttack(0);
        }

        private void OnEnable()
        {
            GameplayEventManager.OnGetAllTraps.AddListener(AddYourselfToTrapsList);
        }

        private void OnDisable()
        {
            GameplayEventManager.OnGetAllTraps.RemoveListener(AddYourselfToTrapsList);
        }

        private void AddYourselfToTrapsList()
        {
            GameController.instance.allTraps.Add(this);
        }

        public virtual void SetStageAttack(int i)
        {
            if(stageNow + i >= stagesAttack.Length)
            {
                StartCoroutine(Attack());
            }
            else {
                stageNow += i;
                animator.SetTrigger(stagesAttack[stageNow]); 
            }
        }

        public virtual bool NextStageIsAttack()
        {
            return stageNow + 1 >= stagesAttack.Length;
        }

        protected virtual IEnumerator Attack()
        {
            yield return new WaitForSeconds(waitTime);
            stageNow = 0;
            SetStageAttack(0);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            GameObject deletObject = null;
            foreach (var attackObject in attackObjects.Where(attackObject => attackObject == other.gameObject))
            {
                deletObject = attackObject;
            }
            if (deletObject != null) attackObjects.Remove(deletObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<TheEssence>() != null)
            {
                attackObjects.Add(other.gameObject);
            }
        }
    }
}
