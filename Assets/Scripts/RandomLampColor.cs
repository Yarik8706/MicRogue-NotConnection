using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RandomLampColor : MonoBehaviour
{
    public Color[] colors;
    private Light2D _light2D;

    [ContextMenu("Test")]
    private void Start()
    {
        _light2D = GetComponent<Light2D>();
        _light2D.color = colors[Random.Range(0, colors.Length)];
    }
}