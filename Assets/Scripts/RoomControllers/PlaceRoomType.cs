using RoomObjects;
using UnityEngine;

namespace RoomControllers
{
    public enum RoomLocation
    {
        Any,
        Center,
        End,
        Between,
        OnlyUpEnd,
        OnlyDownEnd,
        OnlyDownBetween,
        OnlyUpBetween
    }
    
    public class PlaceRoomType : RoomController
    {
        public RoomLocation roomLocation;
        // public GameObject[] mapEnginesType;

        // private void Awake()
        // {
        //     // switch (roomLocation)
        //     // {
        //     //     case RoomLocation.Center:
        //     //         mapObject = mapEnginesType[0];
        //     //         break;
        //     //     case RoomLocation.End:
        //     //         mapObject = mapEnginesType[1];
        //     //         break;
        //     //     case RoomLocation.Between:
        //     //         mapObject = mapEnginesType[2];
        //     //         break;
        //     // }
        // }

        // public override bool CheckCorrectRoom(ExitLocation[] exitLocations, RoomIndex roomIndex)
        // {
        //     re
        //     // if (!base.CheckCorrectRoom(exitLocations, roomIndex)) return false;
        //     // switch (roomLocation)
        //     // {
        //     //     case RoomLocation.Any:
        //     //         return true;
        //     //     case RoomLocation.Center:
        //     //         if ((SpawnLevelController.ShipModel.Length - 1) / 2 == roomIndex.y) return true;
        //     //         break;
        //     //     case RoomLocation.End:
        //     //         if (SpawnLevelController.ShipModel.Length - 1 == roomIndex.y || roomIndex.y == 0) return true;
        //     //         break;
        //     //     case RoomLocation.Between:
        //     //         if (!(SpawnLevelController.ShipModel.Length - 1 == roomIndex.y || roomIndex.y == 0)) return true;
        //     //         break;
        //     //     case RoomLocation.OnlyUpEnd:
        //     //         if (roomIndex.y == 0) return true;
        //     //         break;
        //     //     case RoomLocation.OnlyDownEnd:
        //     //         if (SpawnLevelController.ShipModel.Length - 1 == roomIndex.y) return true;
        //     //         break;
        //     //     default:
        //     //         return false;
        //     // }
        //     // return false;
        // }
    }
}
