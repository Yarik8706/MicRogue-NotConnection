using System;
using Fusion;
using PlayersScripts;
using RoomControllers;
using UnityEngine;

namespace Multiplayer
{
    public class MultiplayerGameRegionControl : MonoBehaviour
    {
        private bool isActive { get; set; }

        [SerializeField] private GameObject bodyRoom;
        [SerializeField] private GameObject[] enemies;
        [SerializeField] private Transform[] enemySpawns;
        [SerializeField] private int enemyCount;

        private int _controlIndex;

        private void Awake()
        {
            bodyRoom.SetActive(false);
        }

        private void Start()
        {
            MultiplayerRegionsControl.instance.multigameRegionControls.Add(this);
            _controlIndex = MultiplayerRegionsControl.instance.multigameRegionControls.IndexOf(this);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if(!col.TryGetComponent(out Player _) || 
               !MultiplayerGameManager.instance.HasStateAuthority || isActive) return;
            isActive = true;
            var enemiesIndexes = RoomController.GetRandomEnemiesIndex(enemies, enemySpawns, enemyCount);
            MultiplayerRegionsControl.instance.RPC_InitialRoom(_controlIndex, enemiesIndexes[0], enemiesIndexes[1]);
        }
        
        public void RPC_InitialRoom(int[] enemiesIndex, int[] enemiesSpawnIndex)
        {
            bodyRoom.SetActive(true);
            for (int i = 0; i < enemiesIndex.Length; i++)
            {
                Instantiate(enemies[enemiesIndex[i]], 
                    enemySpawns[enemiesSpawnIndex[i]].position, Quaternion.identity);
            }
        }
    }
}