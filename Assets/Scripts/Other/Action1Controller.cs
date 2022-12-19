using System;
using Canvas;
using Enemies;
using MainScripts;
using RoomControllers;
using Traps;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Other
{
    public class Action1Controller : MonoBehaviour
    {
        public TrapWall[] trapWalls;
        public Light2D light1;
        public Light2D light2;
        public Dragon dragon;
        public GameObject[] enemySpawns;
        private RoomController _roomController;
        public Transform dragonSpawn;

        private void Start()
        {
            _roomController = GetComponent<RoomController>();
        }

        public void StartAction1()
        {
            dragon = Instantiate(dragon.gameObject, dragonSpawn.position, Quaternion.identity).GetComponent<Dragon>();
            DialogController.instance.StartDialog(DialogController.instance.centerTextDialog, i =>
            {
                switch (i)
                {
                    case 2:
                    {
                        light1.enabled = true;
                        foreach (var trapWall in trapWalls)
                        {
                            trapWall.SetFloorState();
                        }

                        break;
                    }
                    case 3:
                        light2.enabled = true;
                        dragon.WakeUp();
                        break;
                    case -1:
                        _roomController.enemySpawns = enemySpawns;
                        _roomController.SpawnEnemies();
                        GameManager.player.isActive = true;
                        break;
                }
            });
        }
    }
}