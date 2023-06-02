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
        [SerializeField] private Exit upExit;
        [SerializeField] private Exit rightExit;
        [SerializeField] private Exit downExit;
        [SerializeField] private Exit leftExit;
    
        public RoomController Initial()
        {
            var exits = new Exit[4];
            var exitsCount = 0;
            if (HasNearRoom(ExitLocation.Right, roomIndex))
            {
                exitsCount++;
                exits[0] = rightExit;
            }
            if (HasNearRoom(ExitLocation.Left, roomIndex))
            {
                exitsCount++;
                exits[1] = leftExit;
            }
            if (HasNearRoom(ExitLocation.Down, roomIndex))
            {
                exitsCount++;
                exits[2] = downExit;
            }

            if (HasNearRoom(ExitLocation.Up, roomIndex))
            {
                exitsCount++;
                exits[3] = upExit;
            }

            var rightRooms = spawnLevelController.rooms
                .Where(room => room.GetComponent<RoomController>()
                    .CheckCorrectRoom(roomType)).ToList();
            if(rightRooms.Count == 0) return null;
            var newRoom = Instantiate(
                    rightRooms[Random.Range(0, rightRooms.Count)], transform.position, Quaternion.identity)
                .GetComponent<RoomController>();
            newRoom.roomIndex = roomIndex; 
            SpawnLevelController.levelRooms[roomIndex.y][roomIndex.x] = newRoom.gameObject;
            if (exitsCount == 2)
                newRoom.SpawnTwoExits(exits);
            else newRoom.SpawnExits(exits);
            return newRoom;
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
