using System;
using UnityEngine;

namespace RoomObjects
{
    public class SpawnEffectControl : MonoBehaviour
    {
        [SerializeField] private GameObject spawnEffect;

        private void Start()
        {
            Instantiate(spawnEffect, transform.position, Quaternion.identity);
        }
    }
}