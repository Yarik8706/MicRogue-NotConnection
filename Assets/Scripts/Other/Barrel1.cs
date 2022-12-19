using System;
using Enemies;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Other
{
    public class Barrel1 : MonoBehaviour, IFireAttack
    {
        public Sprite brokenBarrel;
        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _boxCollider2D;
        private ShadowCaster2D _shadowCaster2D;

        private void Start()
        {
            _shadowCaster2D = GetComponent<ShadowCaster2D>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void FireDamage(GameObject fire)
        {
            Instantiate(fire, transform.position, Quaternion.identity);
            _boxCollider2D.enabled = false;
            _spriteRenderer.sprite = brokenBarrel;
            _shadowCaster2D.enabled = false;
        }

        public void FireDamage(){}
    }
}