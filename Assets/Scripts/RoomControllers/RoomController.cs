using System;
using System.Collections.Generic;
using System.Linq;
using Enemies;
using JetBrains.Annotations;
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
        [SerializeField] private Transform[] leftExitPositions;
        [SerializeField] private Transform[] rightExitPositions;

        public GameObject[] enemySpawns;
        public RoomType roomType;
        public Transform startPosition;
        
        internal readonly List<Exit> exits = new();
        internal RoomIndex roomIndex;
        
        private GameObject _gameObjects;

        private void Awake()
        {
            _gameObjects = transform.GetChild(0).gameObject;
        }

        // public void SpawnExits(ExitLocation[] exitLocations, Exit exit)
        // {
        //     if(exitPositions.Length == 0) return;
        //     for (int i = 0; i < exitLocations.Length; i++)
        //     {
        //         var newExit = Instantiate(exit, transform);
        //         newExit.transform.position = exitPositions[i].position;
        //         newExit.SetDirectionAndSpriteByDirection(exitLocations[i]);
        //         exits.Add(newExit);
        //     }
        // }

        public void SpawnTwoExits(ExitLocation leftExitLocation, ExitLocation rightExitLocation, Exit exit)
        {
            Transform leftExitTransform;
            Transform rightExitTransform;   
            
            if (leftExitPositions.Length == 1)
            {
                leftExitTransform = leftExitPositions[0];
                rightExitTransform = rightExitPositions[0];
            }
            else
            {   
                leftExitTransform = leftExitPositions[Random.Range(0, leftExitPositions.Length)];
                var leftExitToRightDistances = new List<float>();
                foreach (var rightExitPosition in rightExitPositions)
                {
                    leftExitToRightDistances.Add(Vector2.Distance(rightExitPosition.position, 
                        leftExitTransform.position));
                }
                rightExitTransform = rightExitPositions[
                    leftExitToRightDistances.IndexOf(leftExitToRightDistances.Max())];
            }

            var leftExit = Instantiate(exit, transform);
            var rightExit = Instantiate(exit, transform);
            leftExit.transform.position = leftExitTransform.position;
            rightExit.transform.position = rightExitTransform.position; 
            leftExit.SetDirectionAndSpriteByDirection(leftExitLocation);
            rightExit.SetDirectionAndSpriteByDirection(rightExitLocation);
            exits.Add(leftExit);
            exits.Add(rightExit);
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
        }

        public bool CheckCorrectRoom(RoomType roomType)
        {
            return roomType == this.roomType;
        }

        public void LeavingRoom()
        {
            _gameObjects.SetActive(false);
        }

        public void SpawnEnemies()
        {
            if(enemies.Length == 0 || enemySpawns.Length == 0) return;
            var thisEnemySpawns = enemySpawns.ToList();
            if (thisEnemySpawns.Count == 0) return;
            var roomEnemies = enemies.ToList();
            for (var i = 0; i < enemiesCount;)
            {
                var enemy = roomEnemies[Random.Range(0, roomEnemies.Count)];
                roomEnemies.Remove(enemy);
                var enemySpawn = thisEnemySpawns[Random.Range(0, thisEnemySpawns.Count)];
                Instantiate(enemy, enemySpawn.transform.position, Quaternion.identity);
                thisEnemySpawns.Remove(enemySpawn);
                i += enemy.GetComponent<TheEnemy>().enemyCount;
            }
        }
    }
}