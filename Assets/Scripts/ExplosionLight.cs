using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering.Universal;

public class ExplosionLight : MonoBehaviour
{
    public Light2D explosionLight;
    public float explosionLightIntensity;
    public bool lightInStart = true;

    private void Start()
    {
        if(lightInStart) StartExplosionLight();
    }

    public void StartExplosionLight()
    {
        DOVirtual.Float(0, explosionLightIntensity, .1f, ChangeLight).OnComplete(() => 
                    DOVirtual.Float(explosionLightIntensity, 0, .1f, ChangeLight));
    }

    private void ChangeLight(float x)
    {
        explosionLight.intensity = x;
    }
}