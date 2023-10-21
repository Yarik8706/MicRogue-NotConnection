using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using PlayersScripts;
using RoomControllers;
using RoomObjects;
using UnityEngine;

public enum TheEssenceEffect
{
    None,
    Freezing
}

namespace MainScripts
{
    public class GameManager : MonoBehaviour
    {
        public static Player player;
        public static GameManager instance;
        public static int numberOfMoves;
        public static readonly List<TheEssence> enemyTargets = new();
        
        public RoomController activeRoomController { get; private set; }
        public SpawnLevelController spawnLevelController { get; private set; }
        public ExitLocation lastExitLocation { get; private set; }
        public BackroomsController backroomsController { get; private set; }
        public CameraEffectManager cameraEffectManager { get; private set; }
        
        private GameController _gameController;
        private RoomController[] _roomControllers;
        private TrainingController _trainingController;
        private Transform _cameraTransform;

        [Header("Components")] 
        [SerializeField] private GameObject coldBlackout;
        [SerializeField] private GameObject winFrame;
        public ScreenFader screenFader;
        public GameObject updateShieldObj;
        public int spawnShildCount = 2;

        private void Awake()
        {
            var cameraObject = Camera.main!;
            _cameraTransform = cameraObject.transform;
            cameraEffectManager = GetComponent<CameraEffectManager>();
            backroomsController = GetComponent<BackroomsController>();
            _gameController = GetComponent<GameController>();
            spawnLevelController = GetComponent<SpawnLevelController>();
            _trainingController = GetComponent<TrainingController>();
            instance = this;
        }

        private void Start()
        {
            player = FindObjectOfType<Player>();
            player.gameObject.SetActive(false);
            enemyTargets.Clear();
            enemyTargets.Add(player);
            StartTrainingOrDungeon();
        }

        private void StartTrainingOrDungeon()
        {
            if (PlayerPrefsSafe.GetInt(TrainingController.PrefsTrainingName, 0) == 0)
            {
                SpawnLevelController.activeRooms = spawnLevelController.SpawnRooms(
                    SpawnLevelController.TrainingModel,
                    spawnLevelController.trainingRooms);
                StartCoroutine(_trainingController.Active());
            }
            else
            {
                spawnLevelController.levelRooms = spawnLevelController.SpawnRooms(
                    SpawnLevelController.ShipModel,
                    spawnLevelController.rooms);
                SpawnLevelController.activeRooms = spawnLevelController.levelRooms;
            }
            StartGame(SpawnLevelController.activeRooms[0][0]);
        }

        private void StartGame(RoomController controller)
        {
            cameraEffectManager.ResetGrayEffect();
            StartInitialRoom(controller);
            player.ResetConsumables();
            activeRoomController.SpawnEnemies();
            player.transform.position = activeRoomController.startPosition.position;
        }

        public void StartInitialRoom(RoomController controller)
        {
            activeRoomController = controller;
            activeRoomController.gameObject.SetActive(true);
            activeRoomController.Initial();
            
            _cameraTransform.position = new Vector3(
                activeRoomController.transform.position.x,
                activeRoomController.transform.position.y,
                _cameraTransform.position.z);
            
            screenFader.fadeState = ScreenFader.FadeState.InEnd;
            screenFader.SetState();
            screenFader.fadeState = ScreenFader.FadeState.Out;
        }

        public IEnumerator TurnStarted()
        {
            if (player.essenceEffect == TheEssenceEffect.Freezing && !player.isActive)
            {
                TurnEnded();
                yield break;
            }
            yield return new WaitUntil(() => player.isActive);
            if (player == null) yield break;
            player.Active();
            yield return new WaitUntil(() => player.isTurnOver);
            TurnEnded();
        }

        public void StartWin()
        {
            winFrame.SetActive(true);
        }

        private void TurnEnded()
        {
            GameplayEventManager.OnNextMove.Invoke();
            StartCoroutine(_gameController.ActiveEnemiesAndTraps());
            numberOfMoves++;
        }

        public IEnumerator NextRoom(ExitLocation direction)
        {
            player.isActive = false;
            screenFader.fadeState = ScreenFader.FadeState.In;
            yield return new WaitForSeconds(0.9f);
            
            var indexRoom = GetDirectionIndex(direction, activeRoomController.roomIndex);
            var newRoom = SpawnLevelController.activeRooms[indexRoom.y][indexRoom.x];
            
            if (newRoom == null) yield break;
            
            var directionOfAppearance = direction switch
            {
                ExitLocation.Down => ExitLocation.Up,
                ExitLocation.Left => ExitLocation.Right,
                ExitLocation.Right => ExitLocation.Left,
                ExitLocation.Up => ExitLocation.Down,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };

            newRoom.gameObject.SetActive(true);
            activeRoomController.gameObject.SetActive(false);
            activeRoomController = newRoom;
            activeRoomController.Initial();
            activeRoomController.SpawnEnemies();
            lastExitLocation = directionOfAppearance;
            player.transform.position = activeRoomController.NextPlayerPosition(directionOfAppearance);

            cameraEffectManager.AddGreyEffect();
            screenFader.fadeState = ScreenFader.FadeState.Out;
            player.isActive = true;
            GameplayEventManager.OnNextRoom.Invoke();
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
