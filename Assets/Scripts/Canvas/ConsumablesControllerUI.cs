using System.Collections.Generic;
using UnityEngine;

namespace Canvas
{
    public enum ConsumableType
    {
        Shield,
        Flash
    }
    
    public class ConsumablesControllerUI : MonoBehaviour
    {
        [SerializeField] private ConsumableType consumableType;
        [SerializeField] private GameObject[] consumablesIcons;
        public int RemainingShieldsCount { get; private set; }

        private void Awake()
        {
            if (consumableType == ConsumableType.Shield)
            {
                Player.shieldsControllerUI = this;
            }
            else
            {
                Player.flashCountController = this;
            }
            RemainingShieldsCount = consumablesIcons.Length;
        }

        public void ReduceConsumablesCount()
        {
            consumablesIcons[RemainingShieldsCount-1].SetActive(false);
            // consumablesIconsAnimators[RemainingShieldsCount].Play("LossOfShield");
            RemainingShieldsCount--;
        }

        public void ResetConsumables()
        {
            RemainingShieldsCount = consumablesIcons.Length;
            foreach (var canvasShieldAnimator in consumablesIcons)
            {
                canvasShieldAnimator.SetActive(true);
                // canvasShieldAnimator.Play("CanvasShieldIdle");
            }
        }
    }
}