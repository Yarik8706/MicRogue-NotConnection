using System;
using MainScripts;
using PlayersScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Canvas
{
    public class CustomAbilityController : MonoBehaviour
    {
        public Image activeAbilityButtonImage;

        private bool _isActive = true;

        public static CustomAbilityController instance;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            GameplayEventManager.OnNextRoom.AddListener(() => _isActive = true);
        }

        public void ActiveAbility()
        {
            if(GameManager.player.isTurnOver) return;
            if(!_isActive) return;
            ActiveAbilityPlayer(GameManager.player);
            _isActive = false;
        }

        public void ActiveAbilityPlayer(Player player)
        {
            player.customAbility.ActiveAbility(player);
        }

        [ContextMenu("ResetAbilityActivate")]
        public void ResetAbilityActivate()
        {
            _isActive = true;
        }
    }
}