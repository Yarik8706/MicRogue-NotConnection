﻿using MainScripts;
using UnityEngine;

namespace Canvas
{
    public class CanvasMonsterSpeedController : MonoBehaviour
    {
        public CanvasMonsterSpeedButton slowButton;
        public CanvasMonsterSpeedButton fastButton;

        private void Start()
        {
            if (PlayerPrefsSafe.GetFloat(GameController.EnemyAnimationSpeedKeyName, 1) > 1)
            {
                fastButton.Diseble();
                slowButton.Active();
            }
            else
            {
                fastButton.Active();
                slowButton.Diseble();
            }
        }
    }
}