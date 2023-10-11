using System.Collections.Generic;
using System.Linq;
using RoomControllers;
using UnityEngine;

namespace MainScripts
{
    public enum RoomType
    {
        Null,
        Base,
        Middle,
        Hard,
        VeryHard,
        Action1,
        Action2,
        Start,
        Training1,
        Backrooms,
        BackroomsEnd
    }

    public class SpawnLevelController : MonoBehaviour
    {
        public static RoomController[][] activeRooms;
        public RoomController[][] levelRooms;
        public RoomController[][] activeBackrooms;

        public RoomController[] rooms;
        public RoomController[] trainingRooms;
        public RoomController[] backrooms;
        
        [SerializeField] private GameObject roomSpawn;

        public static readonly RoomType[][] TrainingModel = {
            new []
            {
                RoomType.Start,
                RoomType.Training1
            },
        };
        public static readonly RoomType[][] BackroomsModel = {
            new [] {RoomType.Backrooms, RoomType.Backrooms, RoomType.Backrooms, RoomType.Backrooms, RoomType.Backrooms},
            new [] {RoomType.Backrooms, RoomType.Backrooms, RoomType.Backrooms, RoomType.Null},
            new [] {RoomType.Null, RoomType.Backrooms, RoomType.Backrooms, RoomType.Backrooms, RoomType.Null},
            new [] {RoomType.BackroomsEnd, RoomType.Backrooms, RoomType.Backrooms, RoomType.Backrooms, RoomType.Null},
            new [] {RoomType.Null, RoomType.Null, RoomType.Backrooms, RoomType.Backrooms, RoomType.BackroomsEnd}
        };
        public static readonly RoomType[][] ShipModel = {
            new []{
                RoomType.Start,   
                RoomType.Base,     
                RoomType.Base,     
                RoomType.Middle, 
                RoomType.Middle, 
                RoomType.Hard, 
                RoomType.Action1, 
                RoomType.Hard, 
                RoomType.VeryHard, 
                RoomType.Hard,
                RoomType.VeryHard,
                RoomType.Hard,
                RoomType.VeryHard,  
                RoomType.Action2
            },
        };

        public RoomController[][] SpawnRooms(RoomType[][] mapRoom, RoomController[] centerRooms)
        {
            var allMapRooms = new List<List<RoomController>> { };
            for (var i = 0; i < mapRoom.Length; i++)
            {
                allMapRooms.Add(new List<RoomController>());
                for (var j = 0; j < mapRoom[i].Length; j++)
                {
                    switch (mapRoom[i][j])
                    {
                        case RoomType.Base:
                            goto default;
                        case RoomType.Null:
                            allMapRooms[i].Add(null);
                            break;
                        default:
                            var room = Instantiate(
                                roomSpawn,
                                Vector3.zero, // new Vector2(j * 15, -(i * 9)),
                                Quaternion.identity).GetComponent<RoomSpawn>();
                            room.roomIndex = new RoomIndex(i, j);
                            room.roomType = mapRoom[i][j];
                            
                            var newRoom = room.Initial(centerRooms, mapRoom);
                            newRoom.gameObject.SetActive(false);
                            Destroy(room.gameObject);
                        
                            allMapRooms[i].Add(newRoom);
                        
                            
                            break;
                    }
                }
            }

            return allMapRooms.Select(room => room.ToArray()).ToArray();
        }
    }
}