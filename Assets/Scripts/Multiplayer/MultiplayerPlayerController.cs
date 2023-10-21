using System.Linq;
using Canvas;
using Enemies;
using Fusion;
using MainScripts;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Multiplayer
{
    public class MultiplayerPlayerController : NetworkBehaviour
    {
        [SerializeField] private Light2D smallLight;
        [SerializeField] private Color[] colors;
        
        [Networked] public bool isTurnOver { get; set; } = true;
        [Networked] private bool isLightActive { get; set; } = true;
        [Networked] public int shieldCount { get; set; } = 3;
        [Networked] public int colorIndex { get; private set; }
        
        private NetworkObject _networkObject;
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
                _networkObject = GetComponent<NetworkObject>();
                colorIndex = MultiplayerLobbyManager.instance.spawnedCharacters.Keys.ToList()
                    .IndexOf(_networkObject.InputAuthority);
                MultiplayerGameManager.OnGetAllPlayers.AddListener(() =>
                {
                    if(!_multiplayerPlayer.isActive) return;
                    MultiplayerGameManager.instance.multiplayerPlayerControllers.Add(this);
                });
                MultiplayerGameManager.instance.StartGame();
            }
            else
            {
                RPC_UpdatePosition();
            }
        }

        private void Update()
        {
            _multiplayerPlayer.spriteRenderer.material.SetColor(
                Shader.PropertyToID("_GlowTextureColor"), colors[colorIndex]);

            if (HasInputAuthority && !isLightActive) smallLight.enabled = true;
            else smallLight.enabled = false;

            _multiplayerPlayer.BaseChangeCenterLightActive(isLightActive);
            _multiplayerPlayer.isTurnOver = isTurnOver;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
        public void RPC_Active()
        {
            _multiplayerPlayer.Active();
        }
        
        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_TurnOver()
        {
            if(HasStateAuthority) isTurnOver = true;
            _multiplayerPlayer.BaseTurnOver();
        }
        
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_ChangeCenterLightActive(bool isActive)
        {
            isLightActive = isActive;
        }
        
        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_LightFlash()
        {
            _multiplayerPlayer.BaseLightFlash();
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_Flip(bool turnedRight)
        {
            _multiplayerPlayer.BaseFlip(turnedRight);
        }
        
        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_ActivateProtect()
        {
            _multiplayerPlayer.BaseActivateProtect();
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_DisableProtect()
        {
            _multiplayerPlayer.BaseDisableProtect();
        }
        
        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_Move(Vector3 nextPosition)
        {
            _multiplayerPlayer.BaseMove(nextPosition);
        }

        public void Restore()
        {
            _multiplayerPlayer.RestorationOfShields();
            if (HasInputAuthority) SetCameraPlayerPosition.instance.SetCameraPositionPlayerPosition();
            if (HasStateAuthority)
            {
                MultiplayerGameManager.instance.multiplayerPlayerControllers.Add(this);
                shieldCount = 3;
                RPC_SetPosition(MultiplayerGameManager.instance.GetRespawnPosition(_multiplayerPlayer.blockingLayer));
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_SetPosition(Vector3 newPosition)
        {
            transform.position = newPosition;
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_StealAbility(Vector2 enemyPosition)
        {
            _multiplayerPlayer.BaseStealAbility(GameplayEventManager.GetAllEnemies()
                .Where(enemy => (Vector2)enemy.transform.position == enemyPosition).ToArray()[0].GetComponent<TheEnemy>());
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_UpdatePosition()
        {
            RPC_SetPosition(transform.position);
        }
    }
}