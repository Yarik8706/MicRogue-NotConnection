
using System;

namespace PlayersScripts
{
    [Serializable]
    public struct AbilityDelayControl
    {
        private int _abilityDelayNow;
        
        public int abilityDelay;

        public int GetAbilityDelay()
        {
            return _abilityDelayNow;
        }

        public void ReduceDelay()
        {
            _abilityDelayNow--;
        }

        public void ResetDelay()
        {
            _abilityDelayNow = abilityDelay;
        }
    }
}
