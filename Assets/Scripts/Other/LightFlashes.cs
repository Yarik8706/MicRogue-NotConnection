using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Other
{
    public class LightFlashes : MonoBehaviour
    {
        [SerializeField] private float maxWaitingTime = 3;
        [SerializeField] private float minWaitingTime = 1.5f;
        [SerializeField] private float[] lightValues;
        [SerializeField] private Light2D light2D;
        
        private float _waitTimeNow;
        private float _basePointLightOuterRadius;
        
        private void Start()
        {
            _basePointLightOuterRadius = light2D.pointLightOuterRadius;
        }

        private void Update()
        {
            _waitTimeNow -= Time.deltaTime;
            if (!(_waitTimeNow <= 0)) return;
            LightFlash();
            _waitTimeNow = Random.Range(minWaitingTime, maxWaitingTime);
        }

        private void LightFlash()
        {
            var randomIntensity = lightValues[Random.Range(0, lightValues.Length)];
            ResetAndChangeLightIntensity(_basePointLightOuterRadius + randomIntensity);
            // DOVirtual.Float(GetLightIntensity(), GetLightIntensity() + randomIntensity, 
            //     firstDuration, ChangeLightIntensity).OnComplete(() => 
            //     DOVirtual.Float(GetLightIntensity(),
            //         GetLightIntensity() - randomIntensity, secondDuration, ChangeLightIntensity));
        }
        
        private void ResetAndChangeLightIntensity(float x)
        {
            light2D.pointLightOuterRadius = x;
        }
    }
}