using System;
using MainScripts;
using PlayersScripts;
using UnityEngine;

namespace Canvas
{
    public class PlayerFlashControl : MonoBehaviour
    {
        public void MakeFlash()
        {
            if(GameManager.player.isTurnOver || GameManager.player.flashDelayControl.GetAbilityDelay() > 0) return;
            if(Player.shieldsControllerUI.RemainingShieldsCount == 0) return;
            Player.shieldsControllerUI.ReduceConsumablesCount();
            GameManager.player.LightFlash();
        }
    }
}