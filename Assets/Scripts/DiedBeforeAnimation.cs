using MainScripts;
using UnityEngine;

public class DiedBeforeAnimation : MonoBehaviour
{
    [SerializeField] private GameObject diedObject;
    
    public void Died()
    {
        Destroy(diedObject);
    }
}