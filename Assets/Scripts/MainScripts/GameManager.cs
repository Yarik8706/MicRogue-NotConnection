using System;
using System.Collections;
using RoomControllers;
using RoomObjects;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public enum TheEssenceEffect
{
    None,
    Freezing
}

namespace MainScripts
{
    public class GameManager : MonoBehaviour
    {
        internal bool moveToTheNextRoom;
        internal bool isFindRoom;
        internal Exit lastExit;

        public static Player player;
        public static GameManager instance;
        public static CameraShake cameraShake;
        public static int numberOfMoves;
        
        private GameController _gameController;
        private RoomController _activeRoomController;
        private RoomController[] _roomControllers;
        private SpawnLevelController _spawnLevelController;
        private TrainingController _trainingController;
        private Transform _cameraTransform;
        // Camera Effects ------------------------------------------
        private ColorAdjustments _colorAdjustments;
        private Tonemapping _tonemapping;
        private ChromaticAberration _chromaticAberration;
        private LensDistortion _lensDistortion;
        private SplitToning _splitToning;
        private ShadowsMidtonesHighlights _shadowsMidtonesHighlights;

        [Header("Components")] 
        public GameObject coldBlackout;
        public Volume cameraVolume;
        public ParticleSystem pixelEffect;
        public ScreenFader screenFader;
        public GameObject winFrame;
        public GameObject updateShieldObj;
        public int spawnShildCount = 2;

        private void Awake()
        {
            var cameraObject = FindObjectOfType<Camera>();
            cameraShake = cameraObject.GetComponent<CameraShake>();
            _cameraTransform = cameraObject.transform;
            SetCameraComponents();
        }

        private void Start()
        {
            instance = this;
            _gameController = GetComponent<GameController>();
            _spawnLevelController = GetComponent<SpawnLevelController>();
            _trainingController = GetComponent<TrainingController>();
            
            if (PlayerPrefsSafe.GetInt(TrainingController.PrefsTrainingName, 0) == 0)
            {
                SpawnLevelController.levelRooms = _spawnLevelController.SpawnRooms(
                    SpawnLevelController.TrainingModel,
                    _spawnLevelController.trainingRooms);
                StartGame(SpawnLevelController.levelRooms[0][0]);
                StartCoroutine(_trainingController.Active());
            }
            else
            {
                SpawnLevelController.levelRooms = _spawnLevelController.SpawnRooms(
                                SpawnLevelController.ShipModel,
                                _spawnLevelController.rooms);
                StartGame(SpawnLevelController.levelRooms[0][0]);
            }
        }

        private void StartGame(RoomController controller)
        {
            ResetGrayEffect();
            
            _activeRoomController = controller;
            _activeRoomController.gameObject.SetActive(true);
            _activeRoomController.Initial();

            player.ResetConsumables();
            player.transform.position = _activeRoomController.startPosition.position;
            _activeRoomController.SpawnEnemies();

            _cameraTransform.position = new Vector3(
                _activeRoomController.transform.position.x,
                _activeRoomController.transform.position.y,
                _cameraTransform.position.z);
            
            screenFader.fadeState = ScreenFader.FadeState.InEnd;
            screenFader.SetState();
            screenFader.fadeState = ScreenFader.FadeState.Out;
        }

        [ContextMenu("AddGreyEffect")]
        public void AddGreyEffect()
        {
            var pixelEffectEmission = pixelEffect.emission;
            pixelEffectEmission.rateOverTimeMultiplier += 8;
            var pixelEffectMain = pixelEffect.main;
            pixelEffectMain.simulationSpeed += 0.02f;
            _colorAdjustments.saturation.value -= 9;
            _colorAdjustments.postExposure.value -= 0.01f;
            var shadowsValue = _shadowsMidtonesHighlights.shadows.value;
            shadowsValue.w -= 0.05f;
            _shadowsMidtonesHighlights.shadows.value = shadowsValue;
            
            var midtonesValue = _shadowsMidtonesHighlights.midtones.value;
            midtonesValue.w += 0.1f;
            _shadowsMidtonesHighlights.midtones.value = midtonesValue;
            switch (_activeRoomController.roomType)
            { 
                case RoomType.Null:
                    break;
                case RoomType.Base:
                    break;
                case RoomType.Middle:
                    _chromaticAberration.active = true;
                    break;
                case RoomType.Hard:
                    _splitToning.active = true;
                    break;
                case RoomType.VeryHard:
                    _lensDistortion.active = true;
                    break;
                case RoomType.Action1:
                    // player.GetComponentInChildren<Light2D>().lightOrder = 32;
                    _tonemapping.active = true;
                    break;
                case RoomType.Action2:
                    break;
                case RoomType.Start:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [ContextMenu("ResetGrayEffect")]
        public void ResetGrayEffect()
        {
            var pixelEffectEmission = pixelEffect.emission;
            pixelEffectEmission.rateOverTimeMultiplier = 20;
            var pixelEffectMain = pixelEffect.main;
            pixelEffectMain.simulationSpeed = 0.1f;
            _colorAdjustments.saturation.value = 20;
            _colorAdjustments.postExposure.value = 0;
            
            var shadowsValue = _shadowsMidtonesHighlights.shadows.value;
            shadowsValue.w = 0;
            _shadowsMidtonesHighlights.shadows.value = shadowsValue;
            
            var midtonesValue = _shadowsMidtonesHighlights.midtones.value;
            midtonesValue.w = 0;
            _shadowsMidtonesHighlights.midtones.value = midtonesValue;
            
            _lensDistortion.active = false;
            _tonemapping.active = false;
            _splitToning.active = false;
            _chromaticAberration.active = false;
        }

        public IEnumerator TurnStarted()
        {
            // проверяем не перешел ли игрок в другую комнату если да то ждем когда она загрузится
            yield return new WaitUntil(() => player.isActive);
            if (player == null) yield break;
            player.Active();
            yield return new WaitUntil(() => player.isTurnOver);
            yield return TurnEnded();
        }

        public void StartWin()
        {
            winFrame.SetActive(true);
        }

        public IEnumerator TurnEnded()
        {
            GameplayEventManager.OnNextMove.Invoke();
            yield return new WaitForSeconds(0.3f);
            StartCoroutine(_gameController.ActiveEnemiesAndTraps());
            numberOfMoves++;
        }

        public IEnumerator NextRoom(ExitLocation direction)
        {
            moveToTheNextRoom = true;
            screenFader.fadeState = ScreenFader.FadeState.In;
            yield return new WaitForSeconds(0.4f);
            AddGreyEffect();
            var indexRoom = GetDirectionIndex(direction, _activeRoomController.roomIndex);
            var newRoom = SpawnLevelController.levelRooms[indexRoom.y][indexRoom.x];
            if (newRoom == null) yield break;
            yield return new WaitForSeconds(0.7f);
            
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

            newRoom.gameObject.SetActive(true);
            _activeRoomController.gameObject.SetActive(false);
            _activeRoomController = newRoom;
            _activeRoomController.Initial();
            _activeRoomController.SpawnEnemies();
            foreach (var exit in _activeRoomController.exits)
            {
                if (exit.exitLocation != directionOfAppearance) continue;
                player.transform.position = exit.GetNextPositionPlayer();
                lastExit = exit;
                exit.isActive = false;
            }

            _cameraTransform.position = new Vector3(_activeRoomController.transform.position.x,
                _activeRoomController.transform.position.y, _cameraTransform.position.z);

            screenFader.fadeState = ScreenFader.FadeState.Out;
            moveToTheNextRoom = false;
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
        
        private void SetCameraComponents()
        {
            foreach (var volumeComponent in cameraVolume.sharedProfile.components)
            {
                switch (volumeComponent)
                {
                    case ColorAdjustments colorAdjustments:
                        _colorAdjustments = colorAdjustments;
                        break;
                    case Tonemapping tonemapping:
                        _tonemapping = tonemapping;
                        break;
                    case SplitToning splitToning:
                        _splitToning = splitToning;
                        break;
                    case ShadowsMidtonesHighlights shadowsMidtonesHighlights:
                        _shadowsMidtonesHighlights = shadowsMidtonesHighlights;
                        break;
                    case LensDistortion lensDistortion:
                        _lensDistortion = lensDistortion;
                        break;
                    case ChromaticAberration chromaticAberration:
                        _chromaticAberration = chromaticAberration;
                        break;
                }
            }
        }

        public void ActivateColdBlackout()
        {
            StartCoroutine(ActivateColdBlackoutCoroutine());
        }

        private IEnumerator ActivateColdBlackoutCoroutine()
        {
            coldBlackout.SetActive(true);
            yield return new WaitForSeconds(0.7f);
            coldBlackout.SetActive(false);
        }
    }
}
