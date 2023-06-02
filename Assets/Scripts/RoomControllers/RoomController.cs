using System;
using System.Collections.Generic;
using System.Linq;
using Enemies;
using MainScripts;
using RoomObjects;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace RoomControllers
{
    public struct RoomIndex
    {
        public readonly int y;
        public readonly int x;

        public RoomIndex(int y, int x)
        {
            this.x = x;
            this.y = y;
        }
    }

    public interface IRoomAddiction
    {
        public RoomController RoomController{ get; set; }
    }

    public class RoomController : MonoBehaviour
    {
        [SerializeField] private int enemiesCount;
        [SerializeField] private int spawnChanceShield = 8;
        [SerializeField] private Transform shildSpawnPosition;
        // [SerializeField] private Transform[] lightSpawns;
        // [SerializeField] private int lightsCount;
        [SerializeField] private GameObject[] enemies;
        [Header("Exit Positions: Right, Left, Down, Up")]
        [SerializeField] private Transform[] exitPositions;
        
        public GameObject[] enemySpawns;
        public RoomType roomType;
        public Transform startPosition;
        
        internal readonly List<Exit> exits = new();
        internal RoomIndex roomIndex;
        
        private GameObject _gameObjects;
        // private List<GameObject> _activeLights;

        private void Awake()
        {
            _gameObjects = transform.GetChild(0).gameObject;
        }

        public void SpawnExits(Exit[] newExits)
        {
            if(exitPositions.Length == 0) return;
            for (int i = 0; i < newExits.Length; i++)
            {
                if(newExits[i] == null) continue;
                var exit = Instantiate(newExits[i], transform);
                exit.transform.position = exitPositions[i].transform.position;
                exits.Add(exit);
            }
        }

        public void SpawnTwoExits(Exit[] newExits)
        {
            Vector3 firstExitPosition = Vector3.zero;
            foreach (var newExit in newExits)
            {
                if(newExit == null) continue;
                var exit = Instantiate(newExit, transform);
                if (firstExitPosition != Vector3.zero)
                {
                    if (Vector2.Distance(firstExitPosition, exitPositions[2].position)
                        > Vector2.Distance(firstExitPosition, exitPositions[3].position))
                        exit.transform.position = exitPositions[3].position;
                    else
                        exit.transform.position = exitPositions[2].position;
                } 
                else exit.transform.position = exitPositions[Random.Range(0, 2)].transform.position;
                firstExitPosition = exit.transform.position;
                exits.Add(exit);
            }
        }

        public void Initial()
        {
            _gameObjects.SetActive(true);
            if(shildSpawnPosition == null || GameManager.instance.spawnShildCount == 0) return;
            if (Random.Range(0, spawnChanceShield) == 0)
            {
                GameManager.instance.spawnShildCount--;
                Instantiate(GameManager.instance.updateShieldObj, shildSpawnPosition.position, Quaternion.identity);
            }
            // if(_activeLights.Count != 0)
            // {
            //     while (_activeLights.Count != 0)
            //     {    
            //         var activeLight = _activeLights[0];
            //         _activeLights.Remove(activeLight);
            //         Destroy(activeLight);
            //     }
            // }
            // if (lightsCount == 0) return;
            // var randomSpawnPositions = new List<Transform>{Capacity = lightsCount};
            // for (int i = 0; i < lightsCount; i++)
            // {
            //     var lightPosition = lightSpawns[Random.Range(0, lightSpawns.Length)];
            //     if (randomSpawnPositions.Contains(lightPosition))
            //     {
            //         i--;
            //         continue;
            //     }
            //     randomSpawnPositions.Add(lightPosition);
            //     _activeLights.Add(
            //         Instantiate(GameController.instance.roomLight, lightPosition, true));
            // }
        }

        public bool CheckCorrectRoom(RoomType roomType)
        {
            return roomType == this.roomType;
        }

        public void LeavingRoom()
        {
            _gameObjects.SetActive(false);
        }

        public  void SpawnEnemies()
        {
            if(enemies.Length == 0 || enemySpawns.Length == 0) return;
            var thisEnemySpawns = enemySpawns.ToList();
            for (var i = 0; i < enemiesCount;)
            {
                var enemy = enemies[Random.Range(0, enemies.Length)];
                if (thisEnemySpawns.Count == 0) return;
                var enemySpawn = thisEnemySpawns[Random.Range(0, thisEnemySpawns.Count)];
                Instantiate(enemy, enemySpawn.transform.position, Quaternion.identity);
                thisEnemySpawns.Remove(enemySpawn);
                i += enemy.GetComponent<TheEnemy>().enemyCount;
            }
        }
    }
}