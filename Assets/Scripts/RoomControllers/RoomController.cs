using System;
using System.Collections.Generic;
using System.Linq;
using Enemies;
using MainScripts;
using RoomObjects;
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
        [HideInInspector]public GameObject gameObjects;
        public RoomType roomType;
        public Transform startPosition;
        public Transform[] lightSpawns;
        public int lightsCount;
        private List<GameObject> _activeLights;
        public RoomIndex roomIndex;
        public GameObject[] enemies;
        public GameObject[] enemySpawns;
        public GameObject mapObject;
        public Exit[] exits;
        public int enemiesCount;
        public int spawnChanceShield = 8;
        public Transform shildSpawnPosition;

        private void Awake()
        {
            gameObjects = GetComponentInChildren<Transform>().gameObject;
        }

        public virtual void Initial()
        {
            //TODO: сделать появление щита
            gameObjects.SetActive(true);
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

        public virtual bool CheckCorrectRoom(ExitLocation[] exitLocations, RoomIndex roomIndex, RoomType roomType)
        {
            if (roomType != this.roomType) return false;
            var roomExitLocations = exits.Select(exit => exit.exitLocation).ToArray();
            Array.Sort(roomExitLocations);
            return roomExitLocations.SequenceEqual(exitLocations);
        }

        public virtual void LeavingRoom()
        {
            gameObjects.SetActive(false);
        }

        public virtual void SpawnEnemies()
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

        public virtual void ChangeActiveMapObjects(bool isMapCamera)
        {
            gameObjects.SetActive(!isMapCamera);
        }
    }
}