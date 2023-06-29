using System;
using System.Collections;
using Enemies;
using MainScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Canvas
{
    public class StealAbilityController : MonoBehaviour
    {
        [SerializeField] private GameObject getAbilityAnimationObject;
        [SerializeField] private GameObject putAbilityAnimationObject;
        [SerializeField] private Sprite baseAbilityButtonSprite;
        [SerializeField] private Sprite activeAbilityButtonSprite;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private Image abilityButtonIcon;
        [SerializeField] private CustomAbilityController customAbilityController;
        [SerializeField] private float useRangeAbilitySteal;

        private bool _isActive = true;
        private bool _isSteal; 

        private void Start()
        {
            GameplayEventManager.OnNextRoom.AddListener(() =>
            {
                _isActive = true;
            });
        }

        private void Update()
        {
            if(!_isSteal) return;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider == null
                || !hit.collider.gameObject.TryGetComponent<TheEnemy>(out var enemy)) return;
            if (enemy.enemyAbility == null || Vector2.Distance(
                    enemy.transform.position, 
                    GameManager.player.transform.position
                    ) > useRangeAbilitySteal) return;
            abilityButtonIcon.sprite = baseAbilityButtonSprite;
            _isSteal = false;
            StartCoroutine(StealAbilityCoroutine(enemy));
        }

        private IEnumerator StealAbilityCoroutine(TheEnemy enemy)
        {
            Instantiate(getAbilityAnimationObject, enemy.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
            Instantiate(putAbilityAnimationObject, GameManager.player.transform.position, Quaternion.identity);
            customAbilityController.InitialAbility(enemy.enemyAbility);
            enemy.Died();
            yield return new WaitForSeconds(2.1f);
            GameManager.player.Active();
        }

        public void StealAbility()
        {
            if(GameManager.player.isTurnOver) return;
            if (_isSteal)
            {
                abilityButtonIcon.sprite = baseAbilityButtonSprite;
                _isSteal = false;
                GameManager.player.Active();
                return;
            }
            if(!_isActive) return;
            GameManager.player.DeleteAllMoveToPlaces();
            abilityButtonIcon.sprite = activeAbilityButtonSprite;
            _isSteal = true;
            _isActive = false;
        }
    }
}
