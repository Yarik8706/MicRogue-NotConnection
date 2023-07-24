using System;
using Canvas;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies
{
    [Serializable]
    public class Cause
    {
        public string ruText;
        public string enText;

        public string GetCause()
        {
            return TranslateText.language == "EN" ? enText : ruText;
        }
    }
    
    
    public class CauseOfDied : MonoBehaviour
    {
        public Cause[] causeOfDieds;

        public string GetRandomCauses()
        {
            if (causeOfDieds.Length == 1)
            {
                return causeOfDieds[0].GetCause();
            }

            if (causeOfDieds.Length == 0)
            {
                return "DIIIIIEEETH!";
            }

            return causeOfDieds[Random.Range(0, causeOfDieds.Length)].GetCause();
        }
    }
}