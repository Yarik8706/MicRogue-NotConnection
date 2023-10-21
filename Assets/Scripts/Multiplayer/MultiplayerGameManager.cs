using System.Collections;
using System.Collections.Generic;
using Enemies;
using Fusion;
using MainScripts;
using PlayersScripts;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Multiplayer
{
    public class MultiplayerGameManager : NetworkBehaviour
    {
        public Transform[] startPositions;
        public List<MultiplayerPlayerController> multiplayerPlayerControllers;

        [SerializeField] private GameObject winWindow;
        [SerializeField] private TMP_Text winText;

        public static readonly UnityEvent OnGetAllPlayers = new();
        public static MultiplayerGameManager instance;

        private GameController _gameController;
        private int _addGrayEffectTime = 7;
        private CameraEffectManager _cameraEffectManager;
        private int _activeGrayEffectTime;

        private void Awake()
        {
            GameManager.enemyTargets.Clear();
            instance = this;
        }

        private void Start()
        {
            _cameraEffectManager = GetComponent<CameraEffectManager>();
            _activeGrayEffectTime = _addGrayEffectTime;
            _gameController = GetComponent<GameController>();
            if (MultiplayerLobbyManager.runner.IsServer)
            {
                multiplayerPlayerControllers = new List<MultiplayerPlayerController>();
            }
        }

        public void StartGame()
        {
            GetAllPlayers();
            if (multiplayerPlayerControllers.Count == 1)
            {
                StartCoroutine(PlayersTurn());
            }
        }

        private IEnumerator PlayersTurn()
        {
            _activeGrayEffectTime--;
            if (_activeGrayEffectTime == 0)
            {
                RPC_AddGrayEffect();
                _activeGrayEffectTime = _addGrayEffectTime;
            }

            GetAllPlayers();
            while (multiplayerPlayerControllers.Count > 0)
            {
                var player = multiplayerPlayerControllers[0];
                player.isTurnOver = false;
                if (player.GetComponent<Player>().CheckShieldActive())
                {
                    player.RPC_DisableProtect();
                }
                player.RPC_Active();
                yield return new WaitUntil(() =>
                    !multiplayerPlayerControllers.Contains(player) || player.isTurnOver);
                if (!multiplayerPlayerControllers.Contains(player)) continue;
                multiplayerPlayerControllers.RemoveAt(0);
            }

            RPC_ActiveOtherActivitiesTurn();
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_AddGrayEffect()
        {
            _cameraEffectManager.AddGreyEffect();
        }
        
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_ActiveOtherActivitiesTurn()
        {
            StartCoroutine(OtherActivitiesTurn());
        }

        private IEnumerator OtherActivitiesTurn()
        {
            GameplayEventManager.OnNextMove.Invoke();
            _gameController.ActivateTraps();
            yield return new WaitForSeconds(0.5f);
            yield return _gameController.ActiveEnemies();
            yield return new WaitForSeconds(0.5f);
            GameManager.numberOfMoves++;
            if(HasStateAuthority) StartCoroutine(PlayersTurn());
        }
        
        private void GetAllPlayers()
        {
            multiplayerPlayerControllers.Clear();
            OnGetAllPlayers.Invoke();
        }

        public Vector3 GetRespawnPosition(LayerMask blockingLayer)
        {
            foreach (var startPosition in startPositions)
            {
                if (Ninja.CheckEmptyPlace(startPosition.position, blockingLayer))
                {
                    return startPosition.position;
                }
            }

            return startPositions[0].position;
        }
        
        public void PlayerWin(int index)
        {
            string winText = " player win!";
            switch (index)
            {
                case 0:
                    winText = "Yellow " + winText;
                    break;
                case 1:
                    winText = "Green " + winText;
                    break;
                case 3:
                    winText = "Blue " + winText;
                    break;
                case 4:
                    winText = "White " + winText;
                    break;
                default:
                    winText = "Yellow " + winText;
                    break;
            }

            winWindow.SetActive(true);
            this.winText.text = winText;
        }
    }
}

