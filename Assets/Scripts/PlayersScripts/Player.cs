using System.Collections;
using System.Collections.Generic;
using Canvas;
using Enemies;
using MainScripts;
using RoomObjects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

namespace PlayersScripts
{
    public interface IClickToAvailablePosition
    {
        void ClickEvent(GameObject moveToPlacePrefab, Player player);
    } 

    public class Player : TheEssence, IPointerClickHandler, IStuckInSlime, IHaveShields, IFireAttack
    {
        public static ConsumablesControllerUI flashCountController;
        public static ConsumablesControllerUI shieldsControllerUI;
    
        public int turnCount { get; private set; }
        public CustomAbility customAbility { get; private set; }

        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private GameObject moveToPlace;
        [SerializeField] private ExplosionOfLight explosionLight;
        [SerializeField] private Light2D centerLight;
    
        [Header("Player Attributes")] 
        public AbilityDelayControl flashDelayControl;
        public AbilityDelayControl stealAbilityDelayControl;
        public AbilityDelayControl shieldProtectDelayControl;
    
        internal readonly List<GameObject> moveToPlaces = new();

        private bool _isProtectActive;

        public override void Awake()
        {
            base.Awake();
            GameplayEventManager.OnNextRoom.AddListener(() =>
            {
                DeleteAllMoveToPlaces();
                Active();
            });
        }

        public override void Active()
        {
            if (_isProtectActive)
            {
                DisableProtect();
            }
            DeleteAllMoveToPlaces();
            base.Active();
            moveToPlaces.Clear();
            // проверяем врагов на пути и не даем пройти за ними
            foreach (var newVariantPosition in MoveCalculation(VariantsPositionsNow(variantsPositions)))
            {
                boxCollider2D.enabled = false;
                var hit = Physics2D.Linecast(transform.position, newVariantPosition, enemyLayer);
                boxCollider2D.enabled = true;
                if (hit.collider == null)
                {
                    moveToPlaces.Add(Instantiate(moveToPlace, newVariantPosition, Quaternion.identity));
                }
                else if(hit.collider != null)
                {
                    if (hit.collider.gameObject.transform.position != (Vector3)newVariantPosition) continue;
                    moveToPlaces.Add(Instantiate(moveToPlace, newVariantPosition, Quaternion.identity));
                }
            }

            foreach (var place in moveToPlaces)
            { 
                place.GetComponent<MoveToPlace>().player = this;
            }
        }

        protected override Vector2[] MoveCalculation(Vector2[] theVariantsPositions)
        {
            var nowVariantsPositions = new List<Vector2> {Capacity = 0};
            foreach (var newVariantPosition in theVariantsPositions)
            {
                boxCollider2D.enabled = false;
                var hit = Physics2D.Linecast (transform.position, newVariantPosition, blockingLayer);
                boxCollider2D.enabled = true;
                if (hit.collider == null)
                {
                    nowVariantsPositions.Add(newVariantPosition);
                }
                else if(hit.collider != null)
                {
                    if (hit.collider.gameObject.GetComponent<IClickToAvailablePosition>() is {} thisObject)
                    {
                        thisObject.ClickEvent(moveToPlace, this);
                    }
                }
            }
            return nowVariantsPositions.ToArray();
        }

        public virtual void StartMove(Vector3 @where)
        {
            if(!Ninja.CheckEmptyPlace(where, enemyLayer, out var hit))
            {
                var attackEssence = hit.collider.GetComponent<TheEssence>();
                StartCoroutine(AttackPlayer(attackEssence));
                return;
            }
            StartCoroutine(Move(@where));
        }

        public override IEnumerator Move(Vector3 where)
        {
            DeleteAllMoveToPlaces();
            var x = (int)where.x - (int)transform.position.x;
            if(x <= -2 && turnedRight || x >= 2 && !turnedRight)
            {
                Flip();
            }
            else if(x != 0 && (!turnedRight && x >= 1 || turnedRight && x <= -1))
            {
                boxCollider2D.enabled = false;
                var hit = Physics2D.Linecast (transform.position, where, enemyLayer);
                boxCollider2D.enabled = true;
                if (hit.collider != null)
                {
                    Flip();
                }
            }
            yield return base.Move(@where);
        }

        public virtual void LossShield()
        {
            if(_isProtectActive) return;
            StartAnimation("PlayerProtection");
            shieldsControllerUI.ReduceConsumablesCount(); 
            if (shieldsControllerUI.RemainingShieldsCount == 0)
            {
                animator.SetBool("IdleWithoutShield", true);
            }
        }

        public bool CheckShieldActive()
        {
            return _isProtectActive;
        }

        public void RestorationOfShields()
        {
            animator.SetBool("IdleWithoutShield", false);
            if(GameManager.player != this) return;
            shieldsControllerUI.ResetConsumables();
        }

        public void DeleteAllMoveToPlaces()
        {
            foreach(var i in moveToPlaces)
            {
                Destroy(i);
            }
            moveToPlaces.Clear();
        }

        public virtual void OnPointerClick(PointerEventData @event)
        {
            if(isTurnOver || @event.rawPointerPress.name != gameObject.name) return;
            Flip();
            DeleteAllMoveToPlaces();
            TurnOver();
        }

        public override void Died(MonoBehaviour killer)
        {
            if (killer != null && killer.GetComponent<CauseOfDied>() is {} killerMessage)
            {
                DeathMessageUI.deathMessageUI.ShowMessage(killerMessage.GetRandomCauses());
            }
            else
            {
                DeathMessageUI.deathMessageUI.ShowMessage("Diiiieeetthhh!");
            }
            GameplayEventManager.OnPlayerDied.Invoke();
            GameManager.enemyTargets.Remove(this);
            base.Died();
        }

        public virtual void ChangeCenterLightActive(bool isActive)
        {
            centerLight.enabled = isActive;
        }

        public virtual void LightFlash()
        {
            flashDelayControl.ResetDelay();
            explosionLight.StartExplosionLight();
        }

        public void ResetConsumables()
        {
            flashCountController.ResetConsumables();
            RestorationOfShields();
        }

        protected override IEnumerator AttackPlayer(TheEssence attackEssence)
        {
            DeleteAllMoveToPlaces();
            return base.AttackPlayer(attackEssence);
        }

        public void Stuck(SlimeTrap slimeTrap)
        {
            Instantiate(slimeTrap, transform).Initializate(this);
        }

        public override void TurnOver()
        {
            DeleteAllMoveToPlaces();
            flashDelayControl.ReduceDelay();
            stealAbilityDelayControl.ReduceDelay();
            shieldProtectDelayControl.ReduceDelay();
            turnCount++;
            base.TurnOver();
        }

        public virtual void ActivateProtect()
        {
            shieldProtectDelayControl.ResetDelay();
            animator.SetTrigger("ActivateShield");
            animator.SetBool("IsShieldActive", true);
            _isProtectActive = true;
            TurnOver();
        }

        protected virtual void DisableProtect()
        {
            _isProtectActive = false;
            animator.SetTrigger("DisableShield");
            animator.SetBool("IsShieldActive", false);
        }

        public virtual int GetShieldsCount()
        {
            return shieldsControllerUI.RemainingShieldsCount;
        }
    
        public virtual void StealAbility(TheEnemy essence)
        {
            stealAbilityDelayControl.ResetDelay();
            if(customAbility != null) customAbility.DeleteAbility(this);
            customAbility = essence.enemyAbility;
            customAbility.InitialAbility(this);
            if(GameManager.player == this) 
                customAbility.InitialActiveButton(CustomAbilityController.instance.activeAbilityButtonImage);
            StartCoroutine(StealAbilityController.instance.StealAbilityCoroutine(essence, this));
        }

        public void FireDamage(GameObject firePrefab)
        {
            
        }

        public void FireDamage(MonoBehaviour killer)
        {
            if (!CheckShieldActive())
            {
                Died(killer);
            }
        }
    }
}