using System.Collections;
using System.Collections.Generic;
using Canvas;
using Enemies;
using MainScripts;
using RoomObjects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

public interface IClickToAvailablePosition
{
    void ClickEvent(GameObject moveToPlacePrefab, Player player);
} 

public class Player : TheEssence, IPointerClickHandler, IStuckInSlime, IHaveShields
{
    public static ConsumablesControllerUI flashCountController;
    public static ConsumablesControllerUI shieldsControllerUI;
    
    public LayerMask enemyLayer;
    [SerializeField] private GameObject moveToPlace;
    [SerializeField] private ExplosionOfLight explosionLight;
    [SerializeField] private Light2D centerLight;

    [Header("Player Attributes")] 
    public int flashDelay = 3;
    public int stealAbilityDelay = 5;
    
    internal readonly List<GameObject> moveToPlaces = new();

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
        var x = where.x - transform.position.x;
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

    public virtual void LossOfShieldEvent()
    {
        StartAnimation("PlayerProtection");
        shieldsControllerUI.ReduceConsumablesCount(); 
        if (shieldsControllerUI.RemainingShieldsCount == 0)
        {
            animator.SetBool("IdleWithoutShield", true);
        }
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
        if (killer.GetComponent<CauseOfDied>() is {} killerMessage)
        {
            DeathMessageUI.deathMessageUI.ShowMessage(killerMessage.GetRandomCauses());
        }
        else
        {
            DeathMessageUI.deathMessageUI.ShowMessage("Diiiieeetthhh!");
        }
        GameplayEventManager.OnPlayerDied.Invoke();
        base.Died();
    }

    public virtual void ChangeCenterLightActive(bool isActive)
    {
        centerLight.enabled = isActive;
    }

    public virtual void LightFlash()
    {
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

    public virtual int GetShieldsCount()
    {
        return shieldsControllerUI.RemainingShieldsCount;
    }

    public virtual void LossShield()
    {
        LossOfShieldEvent();
    }
}
