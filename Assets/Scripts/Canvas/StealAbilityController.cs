using System.Collections;
using Enemies;
using MainScripts;
using PlayersScripts;
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
        [SerializeField] private Image abilityButtonIcon;
        [SerializeField] private float useRangeAbilitySteal;

        public static StealAbilityController instance;
        
        private bool _isSteal;

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            if(!_isSteal) return;
            var ray = Camera.main!.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider == null
                || !hit.collider.gameObject.TryGetComponent<TheEnemy>(out var enemy)) return;
            if (enemy.enemyAbility == null || Vector2.Distance(
                    enemy.transform.position, 
                    GameManager.player.transform.position
                    ) > useRangeAbilitySteal) return;
            abilityButtonIcon.sprite = baseAbilityButtonSprite;
            _isSteal = false;
            GameManager.player.StealAbility(enemy);
        }

        public IEnumerator StealAbilityCoroutine(TheEnemy enemy, Player player)
        {
            Instantiate(getAbilityAnimationObject, enemy.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
            Instantiate(putAbilityAnimationObject, player.transform.position, Quaternion.identity);
            enemy.Died(this);
            
            yield return new WaitForSeconds(2.1f);
            if (player != GameManager.player) yield break;
            player.Active();
        }

        public void StealAbility()
        {
            if(GameManager.player.isTurnOver || GameManager.player
                   .stealAbilityDelayControl.GetAbilityDelay() > 0) return;
            if (_isSteal)
            {
                abilityButtonIcon.sprite = baseAbilityButtonSprite;
                _isSteal = false;
                GameManager.player.Active();
                return;
            }
            GameManager.player.DeleteAllMoveToPlaces();
            abilityButtonIcon.sprite = activeAbilityButtonSprite;
            _isSteal = true;
        }
    }
}
