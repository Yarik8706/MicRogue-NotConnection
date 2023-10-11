using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Multiplayer
{
    public class MultiplayerLobbyManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private NetworkPrefabRef playerPrefab;

        public static MultiplayerLobbyManager instance;
        public static NetworkRunner runner;
        public readonly Dictionary<PlayerRef, NetworkObject> spawnedCharacters = new();

        private void Start()
        {
            instance = this;
        }

        public void JoinToRoom()
        {
            StartGame(GameMode.Client);
        }

        public void CreateRoom()
        {
            StartGame(GameMode.Host);
        }

        private async void StartGame(GameMode mode)
        {
            // Create the Fusion runner and let it know that we will be providing user input
            runner = gameObject.AddComponent<NetworkRunner>();
            runner.ProvideInput = true;

            // Start or join (depends on gamemode) a session with a specific name
            await runner.StartGame(new StartGameArgs()
            {
                GameMode = mode,
                SessionName = "TestRoom",
                Scene = SceneManager.GetActiveScene().buildIndex + 2,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                spawnedCharacters.Add(player, null);
                Vector3 spawnPosition
                    = MultiplayerGameManager.instance.startPositions[spawnedCharacters.Keys.ToList().IndexOf(player)].position;
                NetworkObject networkPlayerObject =
                    runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);
                spawnedCharacters[player] = networkPlayerObject;
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            // Find and remove the players avatar
            if (spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
            {
                runner.Despawn(networkObject);
                spawnedCharacters.Remove(player);
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,
            byte[] token)
        {
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
        }
    }
}