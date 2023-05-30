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
        [SerializeField] private int enemiesCount;
        [SerializeField] private int spawnChanceShield = 8;
        [SerializeField] private Transform shildSpawnPosition;
        [SerializeField] private RoomType roomType;
        // [SerializeField] private Transform[] lightSpawns;
        // [SerializeField] private int lightsCount;
        [SerializeField] private GameObject[] enemies;
        [SerializeField] private Transform upExitPosition;
        [SerializeField] private Transform downExitPosition;
        [SerializeField] private Transform rightExitPosition;
        [SerializeField] private Transform leftExitPosition;
        
        public GameObject[] enemySpawns;
        public Exit[] exits;
        public Transform startPosition;
        
        internal RoomIndex roomIndex;
        
        private GameObject _gameObjects;
        private List<GameObject> _activeLights;

        private void Awake()
        {
            _gameObjects = GetComponentInChildren<Transform>().gameObject;
        }

        public void ChangeRoomActive(bool active)
        {
            _gameObjects.SetActive(active);
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

        public virtual bool CheckCorrectRoom(RoomType roomType)
        {
            return roomType == this.roomType;
        }

        public virtual void LeavingRoom()
        {
            _gameObjects.SetActive(false);
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
            _gameObjects.SetActive(!isMapCamera);
        }
    }
}