using System;
using Enemies;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Other
{
    public class Barrel : MonoBehaviour, IFireAttack
    {
        private BoxCollider2D _boxCollider2D;
        private SpriteRenderer _spriteRenderer;
        private ShadowCaster2D _shadowCaster2D;
        public Sprite brokenBarrel;

        private void Start()
        {
            _shadowCaster2D = GetComponent<ShadowCaster2D>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void FireDamage(GameObject firePrefab)
        {
            Instantiate(firePrefab, transform.position, Quaternion.identity);
            _boxCollider2D.enabled = false;
            _shadowCaster2D.enabled = false;
            _spriteRenderer.sprite = brokenBarrel;
            gameObject.layer = LayerMask.NameToLayer("Default");
        }

        public void FireDamage(){}
    }
}