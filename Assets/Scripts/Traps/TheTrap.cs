using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Traps
{
    public abstract class TheTrap : MonoBehaviour, ICauseOfDied, IActiveObject
    {
        public float waitTime;
        public int stageNow;
        public string[] stagesAttack;
        public string attack;
        public string[] causeOfDied;
        protected Animator animator;
        public List<GameObject> attackObjects;
        public bool isActive = true;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            attackObjects = new List<GameObject>();
            SetStageAttack(0);
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

        public string GetDeathText()
        {
            return causeOfDied[Random.Range(0, causeOfDied.Length)];
        }
    }
}
