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
        [SerializeField] private Exit exit;
    
        public RoomController Initial(RoomController[] rooms)
        {
            var exitsDirection = new List<ExitLocation>();
            
            if (HasNearRoom(ExitLocation.Left, roomIndex)) exitsDirection.Add(ExitLocation.Left);
            if (HasNearRoom(ExitLocation.Down, roomIndex)) exitsDirection.Add(ExitLocation.Down);
            if (HasNearRoom(ExitLocation.Up, roomIndex)) exitsDirection.Add(ExitLocation.Up);
            if (HasNearRoom(ExitLocation.Right, roomIndex)) exitsDirection.Add(ExitLocation.Right);

            var rightRooms = rooms
                .Where(room => room.CheckCorrectRoom(roomType)).ToList();
            if(rightRooms.Count == 0) return null;
            var newRoom = Instantiate(
                    rightRooms[Random.Range(0, rightRooms.Count)], transform.position, Quaternion.identity)
                .GetComponent<RoomController>();
            newRoom.roomIndex = roomIndex; 
            if (exitsDirection.Count == 2)
            {
                newRoom.SpawnTwoExits(exitsDirection[0], 
                    exitsDirection[1], exit);
            }
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
