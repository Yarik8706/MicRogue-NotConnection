using System.Linq;
using MainScripts;
using RoomControllers;
using UnityEngine;

namespace Canvas
{
    public class CanvasSelectSpawnRoom : MonoBehaviour
    {
        public GameObject targetMarker;
        public RoomController rightMapRooms;
        public GameObject startImage;

        private MapButton _mapButton;

        private void Start()
        {
            _mapButton = FindObjectOfType<MapButton>();
        }

        public void StartSelectRoomSpawn()
        {
            EndSelectRoomSpawn();
            _mapButton.EnableMap();
            rightMapRooms = SpawnLevelController.levelRooms[0][0].GetComponent<RoomController>();
            // targetMarker.transform.position = rightMapRooms.mapObject.transform.position;
        }

        public void EndSelectRoomSpawn()
        {
            GameManager.instance.StartGame(rightMapRooms);
            _mapButton.DisableMap();
        }
    }
}
