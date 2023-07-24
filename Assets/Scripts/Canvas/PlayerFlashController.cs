using System;
using MainScripts;
using UnityEngine;

namespace Canvas
{
    public class PlayerFlashController : MonoBehaviour
    {
        [SerializeField] private ExplosionOfLight explosionLight;
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
            if(GameManager.player.flashCountController.RemainingShieldsCount == 0) return;
            GameManager.player.flashCountController.ReduceConsumablesCount();
            explosionLight.StartExplosionLight();
            _isActive = false;
        }
    }
}