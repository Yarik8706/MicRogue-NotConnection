using UnityEngine;
using Random = UnityEngine.Random;

namespace RoomObjects
{
    public class SpawnObject : MonoBehaviour
    {
        public GameObject spawnObject;
        public bool isRandom;
        public int spawnChance;

        public void Start()
        {
            if (!isRandom)
            {
                Instantiate(spawnObject, transform.position, Quaternion.identity);
            } else if (Random.Range(0, spawnChance) == 0)
            {
                Instantiate(spawnObject, transform.position, Quaternion.identity);
            }
        }
    }
}
