using System.Collections;
using MainScripts;
using RoomObjects;
using UnityEngine;

namespace Enemies
{
    public interface IStuckInSlime
    {
        public void Stuck(SlimeTrap slimeTrap);
    }
    
    public class Slime : TheEnemy
    {
        [SerializeField] private GameObject slimeRoad;
        [SerializeField] private SlimeTrap slimeTrap;

        public override void Died(MonoBehaviour killer)
        {
            if (killer.GetComponent<IStuckInSlime>() is {} stuck)
            {
                stuck.Stuck(slimeTrap);
            }
            base.Died();
        }
        
        public override IEnumerator Move(Vector3 @where)
        {
            StartCoroutine(SpawnSlimeRoad(@where, transform.position));
            return base.Move(@where);
        }

        private IEnumerator SpawnSlimeRoad(Vector3 nextPosition, Vector3 slimePosition)
        { // если ты посмотрел просто знай что это спавнит слизь от слизьня в нужной позиции с нужным поворотом
            var quaternion = Quaternion.identity;
            if (nextPosition.x == slimePosition.x)
            { 
                quaternion = Quaternion.Euler(0,0,90);
            }
            yield return new WaitForSeconds(0.3f);
            Instantiate(
                slimeRoad, 
                new Vector2(
                    (slimePosition.x + nextPosition.x) / 2, 
                    (slimePosition.y + nextPosition.y) / 2), 
                quaternion);
        }
    }
}
