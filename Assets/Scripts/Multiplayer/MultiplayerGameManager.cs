using System.Collections;
using System.Collections.Generic;
using Enemies;
using Fusion;
using MainScripts;
using Multiplayer;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class MultiplayerGameManager : NetworkBehaviour
{
    public Transform[] farmPositions;
    public Transform[] startPositions;
    public NetworkObject gift;
    public List<MultiplayerPlayerController> multiplayerPlayerControllers;
    
    public static readonly UnityEvent OnGetAllPlayers = new();
    public static MultiplayerGameManager instance;
    
    private GameController _gameController;
    private int _spawnGiftTime = 7;

    private int _activeSpawnGiftTime;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _activeSpawnGiftTime = _spawnGiftTime;
        _gameController = GetComponent<GameController>();
        if (MultiplayerLobbyManager.runner.IsServer)
        {
            multiplayerPlayerControllers = new List<MultiplayerPlayerController>();
        }
    }

    public void StartGame()
    {
        if (multiplayerPlayerControllers.Count == 1)
        {
            StartCoroutine(PlayersTurn());
        }
    }

    public IEnumerator PlayersTurn()
    {
        _activeSpawnGiftTime--;
        if (_activeSpawnGiftTime == 0)
        {
            var chestPosition = farmPositions[Random.Range(0, farmPositions.Length)].position;
            var chest = MultiplayerLobbyManager.runner.Spawn(gift);
            RPC_SetChestPosition(chest, chestPosition);
            _activeSpawnGiftTime = _spawnGiftTime;
        }
        GetAllPlayers();
        while (multiplayerPlayerControllers.Count > 0)
        {
            var player = multiplayerPlayerControllers[0];
            if (player.farmingCount == 3)
            {
                player.RPC_Move(Vector3.zero);
                yield break;
            }
            player.isTurnOver = false;
            player.RPC_Active();
            yield return new WaitUntil(() => player.isTurnOver);
            multiplayerPlayerControllers.RemoveAt(0);
        }

        StartCoroutine(OtherActivitiesTurn());
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ActiveEnemies(RpcInfo info = default)
    {
        StartCoroutine(_gameController.ActiveEnemies());
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetChestPosition(NetworkObject networkObject, Vector3 chestPosition, RpcInfo info = default)
    {
        networkObject.transform.position = chestPosition;
    }

    private IEnumerator OtherActivitiesTurn()
    {
        RPC_ActiveTraps();
        yield return new WaitForSeconds(0.5f);
        RPC_ActiveEnemies();
        yield return new WaitForSeconds(1f);
        StartCoroutine(PlayersTurn());
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ActiveTraps()
    {
        _gameController.ActivateTraps();
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
}

