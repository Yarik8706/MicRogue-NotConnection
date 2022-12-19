using System.Collections.Generic;
using System.Linq;
using MainScripts;
using RoomControllers;
using UnityEngine;

public enum RoomType
{
    Null,
    Base,
    Middle,
    Hard,
    VeryHard,
    Action1,
    Action2,
    Start
}

public class SpawnLevelController : MonoBehaviour
{
    public static GameObject[][] levelRooms;
    public GameObject[] rooms;
    public GameObject roomSpawn;
    public Transform mapObjectsContainer;

    public static readonly RoomType[][] ShipModel = {
        new []{RoomType.Start, RoomType.Base, RoomType.Base, RoomType.Base, RoomType.Null, RoomType.Null, RoomType.Null, RoomType.Null},
        new []{RoomType.Null, RoomType.Null, RoomType.Null, RoomType.Middle, RoomType.Hard, RoomType.Null, RoomType.Null, RoomType.Null},
        new []{RoomType.Null, RoomType.Null, RoomType.Null, RoomType.Null, RoomType.Middle, RoomType.Middle, RoomType.Hard, RoomType.Hard},
        new []{RoomType.Null, RoomType.Null,    RoomType.Null,    RoomType.Null,      RoomType.Null, RoomType.Null,     RoomType.Null, RoomType.Action1},
        new []{RoomType.Null, RoomType.Action2, RoomType.VeryHard, RoomType.VeryHard, RoomType.Hard, RoomType.VeryHard, RoomType.Middle, RoomType.Middle}
    };

    public void Initial(RoomType[][] mapRoom)
    {
        var allRooms = new List<List<GameObject>> { };
        var roomSpawns = new List<RoomSpawn> { };
        for (var i = 0; i < mapRoom.Length; i++)
        {
            allRooms.Add(new List<GameObject>());
            for (var j = 0; j < mapRoom[i].Length; j++)
            {
                switch (mapRoom[i][j])
                {
                    case RoomType.Base:
                        goto default;
                    case RoomType.Null:
                        allRooms[i].Add(null);
                        break;
                    default:
                        var room = Instantiate(
                            roomSpawn,
                            new Vector2(j * 15, -(i * 9)),
                            Quaternion.identity).GetComponent<RoomSpawn>();
                        room.roomIndex = new RoomIndex(i, j);
                        room.spawnLevelController = this;
                        room.roomType = mapRoom[i][j];
                        allRooms[i].Add(room.gameObject);
                        
                        roomSpawns.Add(room);
                        break;
                }
            }
        }
        levelRooms = allRooms.Select(room => room.ToArray()).ToArray();
        
        foreach (var centralRoomSpawn in roomSpawns)
        {
            centralRoomSpawn.Initial();
        }
    }
    
    public GameObject SpawnMapObject(RoomIndex roomIndex, GameObject mapObject)
    {
        var gameObject = Instantiate(mapObject, mapObjectsContainer, true);
        gameObject.transform.localPosition = new Vector3(roomIndex.x / 4f, roomIndex.y / -4f, 0);
        return gameObject;
    }
}