using System;
using System.Collections.Generic;
using System.Linq;
using Canvas;
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
        [SerializeField] protected Transform[] leftExitPositions;
        [SerializeField] protected Transform[] rightExitPositions;
        [SerializeField] protected Transform[] downExitPositions;
        [SerializeField] protected Transform[] upExitPositions;
        [SerializeField] protected Transform[] allExitPositions;

        public GameObject[] enemySpawns;
        public RoomType roomType;
        public Transform startPosition;

        internal readonly List<Exit> exits = new();
        internal RoomIndex roomIndex;

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

            InstatiateExit(leftExitLocation, exit, leftExitTransform.position);
            InstatiateExit(rightExitLocation, exit, rightExitTransform.position);
        }

        private void InstatiateExit(ExitLocation exitLocation, Exit exit, Vector3 position)
        {
            var newExit = Instantiate(exit, transform);
            newExit.transform.position = position;
            newExit.SetDirectionAndSpriteByDirection(exitLocation);
            exits.Add(newExit);
        }

        public void SpawnExits(ExitLocation[] exitLocations, Exit exit)
        {
            foreach (var exitLocation in exitLocations)
            {
                switch (exitLocation)
                {
                    case ExitLocation.Down:
                        InstatiateExit(exitLocation, exit, upExitPositions[0].position);
                        break;
                    case ExitLocation.Left:
                        InstatiateExit(exitLocation, exit, leftExitPositions[0].position);
                        break;
                    case ExitLocation.Right:
                        InstatiateExit(exitLocation, exit, rightExitPositions[0].position);
                        break;
                    case ExitLocation.Up:
                        InstatiateExit(exitLocation, exit, downExitPositions[0].position);
                        break;
                }
            }
        }

        public virtual void Initial()
        {
            if (shildSpawnPosition == null || GameManager.instance.spawnShildCount == 0) return;
            if (Random.Range(0, spawnChanceShield) != 0) return;
            GameManager.instance.spawnShildCount--;
            Instantiate(GameManager.instance.updateShieldObj, shildSpawnPosition.position, Quaternion.identity);
        }

        public bool CheckCorrectRoom(RoomType roomType)
        {
            return roomType == this.roomType;
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

        public Vector3 NextPlayerPosition(ExitLocation direction)
        {
            foreach (var exit in exits.Where(exit => exit.exitLocation == direction))
            {
                if(!exit.willNotActive)exit.isActive = false;
                return exit.GetNextPositionPlayer();
            }
            return Vector3.zero;
        }
    }
}