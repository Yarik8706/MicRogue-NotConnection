using System.Collections;
using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Multiplayer
{
    public class MultiplayerPlayer : Player
    {
        [SerializeField] private int diedTime = 4;
        private MultiplayerPlayerController _multiplayerPlayerController;

        private int _activeDiedTime;
        
        protected override void Start()
        {
            base.Start();
            _multiplayerPlayerController = GetComponent<MultiplayerPlayerController>();
        }

        public override void Active()
        {
            if (!spriteRenderer.enabled && _activeDiedTime == 0)
            {
                _activeDiedTime = -1;
                _multiplayerPlayerController.RPC_Restore();
            }

            if (_activeDiedTime > 0)
            {
                _activeDiedTime--;
                TurnOver();
            }
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

        protected override IEnumerator OnTriggerEnter2D(Collider2D other)
        {
            if (isMove 
                && other.gameObject.CompareTag("Gift") 
                && _multiplayerPlayerController.HasStateAuthority
                && Vector2.Distance(other.gameObject.transform.position, transform.position) 
                < 0.7f)
            {
                _multiplayerPlayerController.AddGift(other.gameObject.GetComponent<NetworkObject>());
            }
            return base.OnTriggerEnter2D(other);
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

        public void SetDiedState()
        {
            spriteRenderer.enabled = false;
            boxCollider2D.enabled = false;
        }

        public void SetLifeState()
        {
            spriteRenderer.enabled = true;
            boxCollider2D.enabled = true;
        }

        public override void Died(MonoBehaviour killer)
        {
            StartAnimationTrigger(diedAnimation);
            Instantiate(diedEffect, transform.position, Quaternion.identity);
            TurnOver();
            if (_multiplayerPlayerController.HasStateAuthority) _multiplayerPlayerController.Died();
            SetDiedState();
            _activeDiedTime = diedTime;
        }
    }
}