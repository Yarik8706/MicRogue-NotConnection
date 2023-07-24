using System;
using MainScripts;
using UnityEngine;

namespace Canvas
{
    public class ChangeUIActiveWhenPlayerDied : MonoBehaviour
    {
        [SerializeField] private GameObject[] changeActiveObejct;
        
        private void Awake()
        {
            GameplayEventManager.OnPlayerDied.AddListener(() =>
            {
                foreach (var o in changeActiveObejct)
                {
                    o.SetActive(false);
                }
            });
        }
    }
}