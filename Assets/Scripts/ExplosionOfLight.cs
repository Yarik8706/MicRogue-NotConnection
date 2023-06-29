using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering.Universal;

public class ExplosionOfLight : MonoBehaviour
{
    private Light2D _light;
    [SerializeField] private float explosionLightIntensity;
    [SerializeField] private float duration = .1f;
    [SerializeField] private bool lightInStart = true;

    private void Start()
    {
        _light = GetComponent<Light2D>();
        if(lightInStart) StartExplosionLight();
    }

    public void StartExplosionLight()
    {
        DOVirtual.Float(0, explosionLightIntensity, duration, ChangeLight).OnComplete(() => 
                    DOVirtual.Float(explosionLightIntensity, 0, duration, ChangeLight));
    }

    private void ChangeLight(float x)
    {
        _light.intensity = x;
    }
}