using System;
using System.Collections;
using DG.Tweening;
using MainScripts;
using UnityEngine;

namespace Enemies
{
    public class SlimeRoad : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        private void Start()
        {
            GameplayEventManager.OnNextRoom.AddListener(() =>
            {
                Destroy(gameObject);
            });
            GameplayEventManager.OnNextMove.AddListener(() =>
            {
                StartCoroutine(DisappearanceSlimeRoad());
            });
        }
        
        private IEnumerator DisappearanceSlimeRoad()
        {
            yield return new WaitForSeconds(1f);
            spriteRenderer.DOFade(0, 0.7f).OnKill(() =>
            {
                Destroy(gameObject);
            });
        }
    }
}