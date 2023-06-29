using System;
using MainScripts;
using UnityEngine;

namespace Canvas
{
    public class PlayerFlashController : MonoBehaviour
    {
        [SerializeField] private ConsumablesControllerUI flashCountController;
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
            if(flashCountController.RemainingShieldsCount == 0) return;
            flashCountController.ReduceConsumablesCount();
            explosionLight.StartExplosionLight();
            _isActive = false;
        }
    }
}