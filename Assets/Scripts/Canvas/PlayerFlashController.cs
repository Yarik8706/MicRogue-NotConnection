using System;
using MainScripts;
using UnityEngine;

namespace Canvas
{
    public class PlayerFlashController : MonoBehaviour
    {
        private bool _isActive = true;

        private void Start()
        {
            GameplayEventManager.OnNextRoom.AddListener(() =>
            {
                _isActive = true;
            });
        }

        public void MakeFlash()
        {
            if(GameManager.player.isTurnOver) return;
            if(!_isActive) return;
            if(Player.flashCountController.RemainingShieldsCount == 0) return;
            Player.flashCountController.ReduceConsumablesCount();
            GameManager.player.LightFlash();
            _isActive = false;
        }
    }
}