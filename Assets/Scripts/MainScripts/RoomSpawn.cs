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
        public Exit exit;
    
        public RoomController Initial(RoomController[] rooms, RoomType[][] model)
        {
            var exitsDirection = new List<ExitLocation>();
            
            if (HasNearRoom(ExitLocation.Left, roomIndex, model)) exitsDirection.Add(ExitLocation.Left);
            if (HasNearRoom(ExitLocation.Down, roomIndex, model)) exitsDirection.Add(ExitLocation.Down);
            if (HasNearRoom(ExitLocation.Up, roomIndex, model)) exitsDirection.Add(ExitLocation.Up);
            if (HasNearRoom(ExitLocation.Right, roomIndex, model)) exitsDirection.Add(ExitLocation.Right);

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
            else if(newRoom.roomType != RoomType.Start)
            {
                newRoom.SpawnExits(exitsDirection.ToArray(), exit);
            }
            return newRoom;
        }

        public static bool HasNearRoom(ExitLocation exitLocation, RoomIndex roomIndex, RoomType[][] model)
        {
            var nextRoomIndex = GameManager.GetDirectionIndex(exitLocation, roomIndex);
            try
            {
                return model[nextRoomIndex.y][nextRoomIndex.x] != RoomType.Null;
            }
            catch
            {
                return false;
            }
        }
    }
}
