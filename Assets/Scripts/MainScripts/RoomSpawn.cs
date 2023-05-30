using System;
using System.Collections.Generic;
using System.Linq;
using RoomControllers;
using RoomObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MainScripts
{
    public class RoomSpawn : MonoBehaviour
    {
        internal RoomIndex roomIndex;
        internal RoomType roomType;
        internal SpawnLevelController spawnLevelController;
        [SerializeField] private GameObject upExit;
        [SerializeField] private GameObject rightExit;
        [SerializeField] private GameObject downExit;
        [SerializeField] private GameObject leftExit;
    
        public void Initial()
        {
            var exitLocationsList = new List<ExitLocation>();
            if(HasNearRoom(ExitLocation.Right, roomIndex)) exitLocationsList.Add(ExitLocation.Right);
            if(HasNearRoom(ExitLocation.Left, roomIndex)) exitLocationsList.Add(ExitLocation.Left);
            if(HasNearRoom(ExitLocation.Down, roomIndex)) exitLocationsList.Add(ExitLocation.Down);
            if(HasNearRoom(ExitLocation.Up, roomIndex)) exitLocationsList.Add(ExitLocation.Up);

            var exitLocations = exitLocationsList.ToArray();
            Array.Sort(exitLocations);
            var rightRooms = spawnLevelController.rooms
                .Where(room => room.GetComponent<RoomController>()
                    .CheckCorrectRoom(roomType)).ToList();
            if(rightRooms.Count == 0) return;
            var newRoom = Instantiate(
                    rightRooms[Random.Range(0, rightRooms.Count)], transform.position, Quaternion.identity)
                .GetComponent<RoomController>();
            newRoom.roomIndex = roomIndex; SpawnLevelController.levelRooms[roomIndex.y][roomIndex.x] = newRoom.gameObject;
            Destroy(gameObject);
        }

        public static bool HasNearRoom(ExitLocation exitLocation, RoomIndex roomIndex)
        {
            var nextRoomIndex = GameManager.GetDirectionIndex(exitLocation, roomIndex);
            try
            {
                return SpawnLevelController.ShipModel[nextRoomIndex.y][nextRoomIndex.x] != RoomType.Null;
            }
            catch (IndexOutOfRangeException exception)
            {
                return false;
            }
        }
    }
}
