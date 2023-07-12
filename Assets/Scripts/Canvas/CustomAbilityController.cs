using System;
using MainScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Canvas
{
    public class CustomAbilityController : MonoBehaviour
    {
        [SerializeField] private Image activeAbilityButtonImage;

        private bool _isActive = true;
        private CustomAbility _stealAbility;

        private void Start()
        {
            GameplayEventManager.OnNextRoom.AddListener(() => _isActive = true);
        }

        public void InitialAbility(CustomAbility customAbility)
        {
            if(_stealAbility != null) _stealAbility.DeleteAbility();
            _stealAbility = customAbility;
            _stealAbility.Initial(activeAbilityButtonImage);
        }
        
        public void ActiveAbility()
        {
            if(GameManager.player.isTurnOver) return;
            if(!_isActive) return;
            _stealAbility.ActiveAbility();
            _isActive = false;
        }

        [ContextMenu("ResetAbilityActivate")]
        public void ResetAbilityActivate()
        {
            _isActive = true;
        }
    }
}