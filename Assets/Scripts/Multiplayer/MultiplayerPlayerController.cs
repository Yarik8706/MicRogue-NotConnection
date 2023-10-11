using Fusion;
using MainScripts;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Multiplayer
{
    public class MultiplayerPlayerController : NetworkBehaviour
    {
        [SerializeField] private Light2D smallLight;
        [Networked] private Vector3 position { get; set; }
        [Networked] private bool turnedRight { get; set; }
        [Networked] public bool isTurnOver { get; set; } = true;
        [Networked] public bool isMove { get; set; }
        [Networked] private bool isLightActive { get; set; } = true;
        [Networked] public int shieldCount { get; set; } = 3;
        [Networked] public int farmingCount { get; set; }
        [Networked] public bool isDead { get; set; }
        private MultiplayerPlayer _multiplayerPlayer;

        private void Start()
        {
            _multiplayerPlayer = GetComponent<MultiplayerPlayer>();
            GameManager.enemyTargets.Add(_multiplayerPlayer);
            if (HasInputAuthority)
            {
                GameManager.player = _multiplayerPlayer;
            }
            if (HasStateAuthority)
            {
                MultiplayerGameManager.instance.multiplayerPlayerControllers.Add(this);
                MultiplayerGameManager.OnGetAllPlayers.AddListener(() =>
                {
                    MultiplayerGameManager.instance.multiplayerPlayerControllers.Add(this);
                });
                MultiplayerGameManager.instance.StartGame();
                position = transform.position;
            }
        }

        private void Update()
        {
            if (isDead) _multiplayerPlayer.SetDiedState();
            else _multiplayerPlayer.SetLifeState();
            if (HasInputAuthority && !isLightActive) smallLight.enabled = true;
            else smallLight.enabled = false;
            _multiplayerPlayer.BaseChangeCenterLightActive(isLightActive);
            _multiplayerPlayer.isTurnOver = isTurnOver;
            if(_multiplayerPlayer.turnedRight != turnedRight)_multiplayerPlayer.BaseFlip(turnedRight);
            if(isMove) return;
            _multiplayerPlayer.transform.position = position;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
        public void RPC_Active(RpcInfo info = default)
        {
            _multiplayerPlayer.Active();
        }
        
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_TurnOver(RpcInfo info = default)
        {
            isMove = false;
            isTurnOver = true;
            position = transform.position;
            _multiplayerPlayer.BaseTurnOver();
        }
        
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_ChangeCenterLightActive(bool isActive, RpcInfo info = default)
        {
            isLightActive = isActive;
        }
        
        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_LightFlash(RpcInfo info = default)
        {
            _multiplayerPlayer.BaseLightFlash();
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_Flip(bool turnedRight, RpcInfo info = default)
        {
            if (HasStateAuthority)
            {
                this.turnedRight = turnedRight;
            }
            _multiplayerPlayer.BaseFlip(turnedRight);
        }
        
        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_Move(Vector3 nextPosition, RpcInfo info = default)
        {
            if (HasStateAuthority)
            {
                isMove = true;
            }
            _multiplayerPlayer.BaseMove(nextPosition);
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_Restore(RpcInfo info = default)
        {
            _multiplayerPlayer.SetLifeState();
            _multiplayerPlayer.RestorationOfShields();
            if (HasStateAuthority)
            {
                MultiplayerGameManager.instance.multiplayerPlayerControllers.Add(this);
                position = MultiplayerGameManager.instance.GetRespawnPosition(_multiplayerPlayer.enemyLayer);
                shieldCount = 3;
                isDead = false;
                RPC_SetPositionAndActive(position);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
        private void RPC_SetPositionAndActive(Vector3 newPosition, RpcInfo info = default)
        {
            transform.position = newPosition;
            _multiplayerPlayer.Active();
        }
        
        public void AddGift(NetworkObject gift, RpcInfo info = default)
        {
            farmingCount++;
            MultiplayerLobbyManager.runner.Despawn(gift);
        }

        public void Died()
        {
            isDead = true;
            MultiplayerGameManager.instance.multiplayerPlayerControllers.Remove(this);
        }
    }
}