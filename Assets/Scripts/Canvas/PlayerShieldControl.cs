using MainScripts;
using PlayersScripts;
using UnityEngine;

namespace Canvas
{
    public class PlayerShieldControl : MonoBehaviour
    {
        public void ActivateShield()
        {
            if(GameManager.player.isTurnOver 
               || GameManager.player.shieldProtectDelayControl.GetAbilityDelay() > 0
               || Player.flashCountController.RemainingShieldsCount == 0) return;
            Player.shieldsControllerUI.ReduceConsumablesCount(); 
            GameManager.player.ActivateProtect();
        }
    }
}