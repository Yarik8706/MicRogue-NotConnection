using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MainScripts
{
    public class CameraEffectManager : MonoBehaviour
    {
        [SerializeField] private Volume cameraVolume;
        [SerializeField] private ParticleSystem pixelEffect;
        
        private ColorAdjustments _colorAdjustments;
        private Tonemapping _tonemapping;
        private ChromaticAberration _chromaticAberration;
        private LensDistortion _lensDistortion;
        private SplitToning _splitToning;
        private ShadowsMidtonesHighlights _shadowsMidtonesHighlights;
        private bool _isMaxEffect;

        private void Awake()
        {
            SetCameraComponents();
        }

        public void AddGreyEffect()
        {
            if(_isMaxEffect && !BackroomsController.isBackrooms) return;
            var pixelEffectEmission = pixelEffect.emission;
            pixelEffectEmission.rateOverTimeMultiplier += 8;
            var pixelEffectMain = pixelEffect.main;
            pixelEffectMain.simulationSpeed += 0.02f;
            _colorAdjustments.saturation.value -= 9;
            _colorAdjustments.postExposure.value -= 0.01f;
            var shadowsValue = _shadowsMidtonesHighlights.shadows.value;
            shadowsValue.w -= 0.05f;
            _shadowsMidtonesHighlights.shadows.value = shadowsValue;
            
            var midtonesValue = _shadowsMidtonesHighlights.midtones.value;
            midtonesValue.w += 0.1f;
            _shadowsMidtonesHighlights.midtones.value = midtonesValue;
            switch (GameManager.instance.activeRoomController.roomType)
            { 
                case RoomType.Null:
                    break;
                case RoomType.Base:
                    break;
                case RoomType.Middle:
                    _chromaticAberration.active = true;
                    break;
                case RoomType.Hard:
                    _splitToning.active = true;
                    break;
                case RoomType.VeryHard:
                    _lensDistortion.active = true;
                    _isMaxEffect = true;
                    break;
                case RoomType.Action1:
                    _tonemapping.active = true;
                    break;
                case RoomType.Action2:
                    break;
                case RoomType.Start:
                    break;
                case RoomType.Backrooms:
                    _chromaticAberration.active = true;
                    _tonemapping.active = true;
                    _splitToning.active = true;
                    _lensDistortion.active = true;
                    break;
            }
        }

        [ContextMenu("ResetGrayEffect")]
        public void ResetGrayEffect()
        {
            var pixelEffectEmission = pixelEffect.emission;
            pixelEffectEmission.rateOverTimeMultiplier = 20;
            var pixelEffectMain = pixelEffect.main;
            pixelEffectMain.simulationSpeed = 0.1f;
            _colorAdjustments.saturation.value = 20;
            _colorAdjustments.postExposure.value = 0;
            
            var shadowsValue = _shadowsMidtonesHighlights.shadows.value;
            shadowsValue.w = 0;
            _shadowsMidtonesHighlights.shadows.value = shadowsValue;
            
            var midtonesValue = _shadowsMidtonesHighlights.midtones.value;
            midtonesValue.w = 0;
            _shadowsMidtonesHighlights.midtones.value = midtonesValue;
            
            _lensDistortion.active = false;
            _tonemapping.active = false;
            _splitToning.active = false;
            _chromaticAberration.active = false;
        }
        
        private void SetCameraComponents()
        {
            foreach (var volumeComponent in cameraVolume.sharedProfile.components)
            {
                switch (volumeComponent)
                {
                    case ColorAdjustments colorAdjustments:
                        _colorAdjustments = colorAdjustments;
                        break;
                    case Tonemapping tonemapping:
                        _tonemapping = tonemapping;
                        break;
                    case SplitToning splitToning:
                        _splitToning = splitToning;
                        break;
                    case ShadowsMidtonesHighlights shadowsMidtonesHighlights:
                        _shadowsMidtonesHighlights = shadowsMidtonesHighlights;
                        break;
                    case LensDistortion lensDistortion:
                        _lensDistortion = lensDistortion;
                        break;
                    case ChromaticAberration chromaticAberration:
                        _chromaticAberration = chromaticAberration;
                        break;
                }
            }
        }
    }
}