using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Other
{
    public class LightFlashes : MonoBehaviour
    {
        private float maxWaitingTime = 3;
        private float minWaitingTime = 1.5f;
        public float maxLightIntensity;
        public float minLightIntensity;
        public float firstDuration;
        public float secondDuration;
        public Light2D light2D;
        private float _waitTimeNow;

        private void Update()
        {
            _waitTimeNow -= Time.deltaTime;
            if (!(_waitTimeNow <= 0)) return;
            LightFlash();
            _waitTimeNow = Random.Range(minWaitingTime, maxWaitingTime);
        }

        public float GetLightIntensity()
        {
            return light2D.pointLightInnerRadius;
        }

        private void LightFlash()
        {
            var randomIntensity = Random.Range(minLightIntensity, maxLightIntensity);
            DOVirtual.Float(GetLightIntensity(), GetLightIntensity() + randomIntensity, 
                firstDuration, ChangeLightIntensity).OnComplete(() => 
                DOVirtual.Float(GetLightIntensity(),
                    GetLightIntensity() - randomIntensity, secondDuration, ChangeLightIntensity));
        }
        
        private void ChangeLightIntensity(float x)
        {
            light2D.pointLightInnerRadius = x;
        }
    }
}