using System.Collections.Generic;
using MainScripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RoomControllers
{
    public class BackroomController : RoomController
    {
        [SerializeField] private int maxSpawnWallCount;
        [SerializeField] private List<GameObject> backroomsWalls;
        [SerializeField] private GameObject bacterium;

        private bool isActiveRoom;
        private Vector3 _oldBacteriumPosition;

        public override void Initial()
        {
            if (isActiveRoom && _oldBacteriumPosition != Vector3.zero)
            {
                Instantiate(bacterium, _oldBacteriumPosition, Quaternion.identity);
            }
            
            if(isActiveRoom) return;
            isActiveRoom = true;
            var spawnWallCount = Random.Range(0, maxSpawnWallCount);
            for (int i = 0; i < spawnWallCount; i++)
            {
                var randomIndex = Random.Range(0, backroomsWalls.Count);
                backroomsWalls[randomIndex].SetActive(true);
                backroomsWalls.RemoveAt(randomIndex);
            }
            base.Initial();
        }

        private void OnDisable()
        {
            if(_oldBacteriumPosition == Vector3.zero) return;
            GameplayEventManager.GetAllEnemies();
            if(GameController.instance.allEnemies.Count == 0) return;
            _oldBacteriumPosition = GameController.instance.allEnemies[0].transform.position;
        }

        public void SpawnBacterium()
        {
            var exit = GameManager.player;

            if (rightExitPositions[0].position == exit.transform.position)
            {
                Instantiate(bacterium, leftExitPositions[0].transform.position, Quaternion.identity);
            } 
            else if (leftExitPositions[0].position == exit.transform.position)
            {
                Instantiate(bacterium, rightExitPositions[0].transform.position, Quaternion.identity);
            }
            else if (downExitPositions[0].position == exit.transform.position)
            {
                Instantiate(bacterium, upExitPositions[0].transform.position, Quaternion.identity);
            }
            else if (upExitPositions[0].position == exit.transform.position)
            {
                Instantiate(bacterium, downExitPositions[0].transform.position, Quaternion.identity);
            }
        }
    }
}