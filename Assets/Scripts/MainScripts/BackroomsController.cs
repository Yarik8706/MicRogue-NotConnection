using System.Collections;
using DG.Tweening;
using Other;
using RoomControllers;
using RoomObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MainScripts
{
    public class BackroomsController : MonoBehaviour
    {
        [SerializeField] private AudioSource fallSound;
        [SerializeField] private AudioSource backroomsSound;
        [SerializeField] private GameObject playerLamp;
        [SerializeField] private RoomSpawn roomSpawn;
        [SerializeField] private Exit backroomsExit;
        [SerializeField] private int bacteriumSpawnChance;

        private Exit _oldExit;
        private RoomIndex _lastLevelRoomIndex;
        private int _backroomsRoomCount;

        public static bool isBackrooms;

        private void Awake()
        {
            _oldExit = roomSpawn.exit;
            GameplayEventManager.OnNextRoom.AddListener(() =>
            {
                if (!isBackrooms) return;
                if (_backroomsRoomCount == 3)
                {
                    var randomNumber = Random.Range(0, bacteriumSpawnChance);
                    if(randomNumber != 0) return;
                    var room = (BackroomController) GameManager.instance.activeRoomController;
                    room.SpawnBacterium();
                }

                _backroomsRoomCount++;
            });
        }

        public void StartBackrooms()
        {
            if(isBackrooms) return;
            isBackrooms = true;
            roomSpawn.exit = backroomsExit;
            _lastLevelRoomIndex = GameManager.instance.activeRoomController.roomIndex;
            GameManager.player.isActive = false;
            GameManager.instance.screenFader.fadeState = ScreenFader.FadeState.In;
            GameManager.instance.activeRoomController.gameObject.SetActive(false);
            GameManager.instance.spawnLevelController.activeBackrooms = 
                GameManager.instance.spawnLevelController.SpawnRooms(
                SpawnLevelController.BackroomsModel,
                GameManager.instance.spawnLevelController.backrooms);
            SpawnLevelController.activeRooms = GameManager.instance.spawnLevelController.activeBackrooms;
            GameManager.instance.StartInitialRoom(SpawnLevelController.activeRooms[0][0]);
            StartCoroutine(StartBackroomsCoroutine());
            backroomsSound.Play();
            playerLamp.SetActive(false);
            GameManager.instance.cameraEffectManager.AddGreyEffect();
            if (MusicController.instance != null)
            {
                MusicController.instance.PlayBackroomsMusic();
            }
        }

        private void OnDestroy()
        {
            roomSpawn.exit = _oldExit;
        }

        public IEnumerator ExitBackrooms()
        {
            GameManager.player.isActive = false;
            GameManager.instance.activeRoomController.gameObject.SetActive(false);
            GameManager.instance.cameraEffectManager.ResetGrayEffect();
            GameManager.instance.cameraEffectManager.AddGreyEffect();
            SpawnLevelController.activeRooms = GameManager.instance.spawnLevelController.levelRooms;
            GameManager.instance.StartInitialRoom(SpawnLevelController.activeRooms[_lastLevelRoomIndex.y]
                [_lastLevelRoomIndex.x]);
            GameplayEventManager.OnNextRoom.Invoke();
            GameManager.instance.activeRoomController.SpawnEnemies();
            GameManager.instance.screenFader.fadeState = ScreenFader.FadeState.InEnd;
            GameManager.instance.screenFader.SetState();
            GameManager.instance.screenFader.fadeState = ScreenFader.FadeState.Out;
            GameManager.player.transform.position = GameManager.instance.activeRoomController
                .NextPlayerPosition(ExitLocation.Left);
            GameManager.player.DeleteAllMoveToPlaces();
            playerLamp.SetActive(true);
            roomSpawn.exit = _oldExit;
            GameplayEventManager.OnNextRoom.Invoke();
            MusicController.instance.StopMusicAndPlayNext();
            _backroomsRoomCount = 0;

            yield return new WaitForSeconds(1f);
            
            isBackrooms = false;
            GameManager.player.Active();
            GameManager.player.isActive = true;
        }

        private IEnumerator StartBackroomsCoroutine()
        {
            GameManager.player.transform.position = 
                            GameManager.instance.activeRoomController.startPosition.position + Vector3.up * 10;
            GameplayEventManager.OnNextRoom.Invoke(); 
            yield return new WaitForSeconds(1f);
            GameManager.player.transform.DOMove(
                GameManager.instance.activeRoomController.startPosition.position, 
                0.9f);
            yield return new WaitForSeconds(0.9f);
            StartCoroutine(CameraShake.instance.Shake(0.5f, 1));
            fallSound.Play();
            GameManager.player.isActive = true;
            GameManager.player.Active();
        }
    }
}