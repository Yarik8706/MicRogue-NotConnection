using Enemies;
using PlayersScripts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Multiplayer
{
    public class MultiplayerPlayer : Player
    {
        private MultiplayerPlayerController _multiplayerPlayerController;
        
        public override void Awake()
        {
            base.Awake();
            _multiplayerPlayerController = GetComponent<MultiplayerPlayerController>();
        }
        
        public override void Active()
        {
            if(!_multiplayerPlayerController.HasInputAuthority) return;
            base.Active();
        }

        public override void Flip(bool turnedRight)
        {
            if(!_multiplayerPlayerController.HasInputAuthority) return;
            _multiplayerPlayerController.RPC_Flip(turnedRight);
        }

        public override void StartMove(Vector3 @where)
        {
            _multiplayerPlayerController.RPC_Move(@where);
        }
        
        public void BaseMove(Vector3 @where)
        {
            base.StartMove(where);
        }

        public override void OnPointerClick(PointerEventData @event)
        {
            if(!_multiplayerPlayerController.HasInputAuthority) return;
            base.OnPointerClick(@event);
        }
        
        public override void TurnOver()
        {
            if(!_multiplayerPlayerController.HasInputAuthority) return;
            _multiplayerPlayerController.RPC_TurnOver();
        }
        
        public void BaseTurnOver()
        {
            base.TurnOver();
        }

        public override void ChangeCenterLightActive(bool isActive)
        {
            _multiplayerPlayerController.RPC_ChangeCenterLightActive(isActive);
        }

        public override void LightFlash()
        {
            _multiplayerPlayerController.RPC_LightFlash();
        }

        public void BaseLightFlash()
        {
            base.LightFlash();
        }

        public void BaseChangeCenterLightActive(bool isActive)
        {
            base.ChangeCenterLightActive(isActive);
        }

        public void BaseFlip(bool turnedRight)
        {
            base.Flip(turnedRight);
        }

        public override int GetShieldsCount()
        {
            return _multiplayerPlayerController.shieldCount;
        }

        public override void LossShield()
        {
            if (_multiplayerPlayerController.HasStateAuthority)
            {
                _multiplayerPlayerController.shieldCount--;
            }

            if (_multiplayerPlayerController.HasInputAuthority)
            {
                base.LossShield();
            }
        }

        public override void Died(MonoBehaviour killer)
        {
            StartAnimationTrigger(diedAnimation);
            Instantiate(diedEffect, transform.position, Quaternion.identity);
            TurnOver();
            _multiplayerPlayerController.Restore();
        }

        public override void StealAbility(TheEnemy essence)
        {
            _multiplayerPlayerController.RPC_StealAbility(essence.transform.position);
        }

        public void BaseStealAbility(TheEnemy enemy)
        {
            base.StealAbility(enemy);
        }

        public override void ActivateProtect()
        {
            _multiplayerPlayerController.RPC_ActivateProtect();
        }
        
        public void BaseActivateProtect()
        {
            base.ActivateProtect();
        }

        protected override void DisableProtect()
        {
            if(!_multiplayerPlayerController.HasStateAuthority) return;
            Debug.Log(gameObject.name + " start disable");
            _multiplayerPlayerController.RPC_DisableProtect();
        }
        
        public void BaseDisableProtect()
        {
            base.DisableProtect();
        }
    }
}