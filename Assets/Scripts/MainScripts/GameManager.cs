using System;
using System.Collections;
using System.Linq;
using Canvas;
using RoomControllers;
using RoomObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MainScripts
{
    public class GameManager : MonoBehaviour
    {
        internal bool moveToTheNextRoom;
        internal bool isFindRoom;
        internal Vector3 lastNextExitPosition;

        public static Player player;
        public static GameManager instance;
        public static CameraShake cameraShake;
        
        private GameController _gameController;
        internal RoomController roomController;
        private RoomController[] _roomControllers;
        private SpawnLevelController _spawnLevel;
        private Transform _cameraTransform;

        [Header("Components")] 
        public ScreenFader screenFader;
        public GameObject winFrame;
        public GameObject updateShieldObj;
        public int spawnShildCount = 2;

        private void Awake()
        {
            var cameraObject = FindObjectOfType<Camera>();
            cameraShake = cameraObject.GetComponent<CameraShake>();
            _cameraTransform = cameraObject.transform;
        }

        private void Start()
        {
            instance = this;
            _gameController = GetComponent<GameController>();
            _spawnLevel = GetComponent<SpawnLevelController>();
            _spawnLevel.Initial(SpawnLevelController.ShipModel);
            //сделать выбор комнат на которых спавниться сокровища
            _roomControllers = FindObjectsOfType<RoomController>();
            foreach (var roomController in _roomControllers)
            {
                roomController.ChangeRoomActive(false);
            }
            StartGame(SpawnLevelController.levelRooms[0][0].GetComponent<RoomController>());
        }

        public void StartGame(RoomController controller)
        {
            screenFader.fadeState = ScreenFader.FadeState.In;
            roomController = controller;
            roomController.Initial();
            player.transform.position = roomController.startPosition.position;
            roomController.SpawnEnemies();

            _cameraTransform.position = new Vector3(
                roomController.transform.position.x,
                roomController.transform.position.y,
                _cameraTransform.position.z);
            screenFader.fadeState = ScreenFader.FadeState.Out;
        }

        public IEnumerator TurnStarted()
        {
            GameController.instance.enemiesActive = false;
            // проверяем не перешел ли игрок в другую комнату если да то ждем когда она загрузится
            yield return new WaitUntil(() => player.isActive);
            // проверяем не умер ли игрок
            if (player == null) yield break;
            player.Active();
            yield return new WaitForSeconds(0.1f);
            yield return new WaitUntil(() => player.isTurnOver);
            yield return TurnEnded();
        }

        public void StartWin()
        {
            winFrame.SetActive(true);
        }

        public IEnumerator TurnEnded()
        {
            // активируем событие следущего хода 
            GameplayEventManager.OnNextMove.Invoke();
            yield return new WaitForSeconds(0.3f);
            //активируем врагов и ловушки
            yield return _gameController.Active();
        }

        public IEnumerator NextRoom(ExitLocation direction)
        {
            moveToTheNextRoom = true;
            screenFader.fadeState = ScreenFader.FadeState.In;
            yield return new WaitForSeconds(0.4f);
            var indexRoom = GetDirectionIndex(direction, roomController.roomIndex);
            var newRoom = SpawnLevelController.levelRooms[indexRoom.y][indexRoom.x];
            if (newRoom == null) yield break;
            yield return new WaitForSeconds(0.7f);
            roomController.LeavingRoom();
        
            // TODO: сделать добавление мусора в как дочерний элемент в комнату
            foreach (var afterDied in GameObject.FindGameObjectsWithTag("Trash"))
            {
                Destroy(afterDied);
            }

            var directionOfAppearance = direction switch
            {
                ExitLocation.Down => ExitLocation.Up,
                ExitLocation.Left => ExitLocation.Right,
                ExitLocation.Right => ExitLocation.Left,
                ExitLocation.Up => ExitLocation.Down,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };

            newRoom.SetActive(true);
            roomController = newRoom.GetComponent<RoomController>();
            roomController.Initial();
            roomController.SpawnEnemies();
            foreach (var exit in roomController.exits)
            {
                if (exit.exitLocation == directionOfAppearance)
                {
                    player.transform.position = exit.GetNextPositionPlayer();
                    lastNextExitPosition = player.transform.position;
                }
            }

            _cameraTransform.position = new Vector3(roomController.transform.position.x,
                roomController.transform.position.y, _cameraTransform.position.z);

            screenFader.fadeState = ScreenFader.FadeState.Out;
            moveToTheNextRoom = false;
            GameplayEventManager.OnNextRoom.Invoke();
            
            player.Active();
        }

        public static RoomIndex GetDirectionIndex(ExitLocation direction, RoomIndex roomIndex)
        {
            return direction switch
            {
                ExitLocation.Down => new RoomIndex(roomIndex.y + 1, roomIndex.x), // идет прибавление така как индекс начинатеся с нуля и чтобы идти вниз надо пребавлять
                ExitLocation.Left => new RoomIndex(roomIndex.y, roomIndex.x - 1),
                ExitLocation.Up => new RoomIndex(roomIndex.y - 1, roomIndex.x),
                ExitLocation.Right => new RoomIndex(roomIndex.y, roomIndex.x + 1),
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }
    }
}
