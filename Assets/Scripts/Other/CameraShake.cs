using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    private Vector3 _cameraPosition;
    
    public static CameraShake instance;

    private void Start()
    {
        instance = this;
    }

    public IEnumerator Shake(float time, float magnitube)
    {
        _cameraPosition = transform.position;
        var elapsedTime = 0f;
        while (elapsedTime < time)
        {
            var xPos = Random.Range(-0.5f, 0.5f) * magnitube;
            var yPos = Random.Range(-0.5f, 0.5f) * magnitube;
            transform.position = _cameraPosition + new Vector3(xPos, yPos, _cameraPosition.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = _cameraPosition;
    }
}
