using UnityEngine;

namespace Canvas
{
    public class ConsumablesControllerUI : MonoBehaviour
    {
        [SerializeField] private GameObject[] consumablesIcons;
        public int RemainingShieldsCount { get; private set; }

        private void Start()
        {
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