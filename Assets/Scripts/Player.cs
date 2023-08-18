using System.Collections;
using System.Collections.Generic;
using Canvas;
using Enemies;
using MainScripts;
using RoomObjects;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IClickToAvailablePosition
{
    void ClickEvent(GameObject moveToPlacePrefab, Player player);
} 

public class Player : TheEssence, IPointerClickHandler, IStuckInSlime
{
    public ConsumablesControllerUI flashCountController;
    public ConsumablesControllerUI shieldsControllerUI;
    
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private DeathMesssageUI deathMesssage;
    [SerializeField] private GameObject moveToPlace;
    
    internal List<GameObject> moveToPlaces;
    internal Vector3 movingPosition;

    public override void Awake()
    {
        base.Awake();
        GameManager.player = this;
        moveToPlaces = new List<GameObject>();
        gameObject.SetActive(false);
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

    public void StartMove(Vector3 @where)
    {
        StartCoroutine(Move(@where));
    }

    public override IEnumerator Move(Vector3 where)
    {
        movingPosition = where;
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

    public void LossOfShieldEvent()
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
        shieldsControllerUI.ResetConsumables();
        animator.SetBool("IdleWithoutShield", false);
    }

    public void DeleteAllMoveToPlaces()
    {
        foreach(var i in moveToPlaces)
        {
            Destroy(i);
        }
    }

    public void OnPointerClick(PointerEventData @event)
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
            deathMesssage.ShowMessage(killerMessage.GetRandomCauses());
        }
        else
        {
            deathMesssage.ShowMessage("Diiiieeetthhh!");
        }
        GameplayEventManager.OnPlayerDied.Invoke();
        base.Died();
    }

    public void ResetConsumables()
    {
        flashCountController.ResetConsumables();
        RestorationOfShields();
    }
    
    public void Stuck(SlimeTrap slimeTrap)
    {
        Instantiate(slimeTrap, transform).Initializate(this);
    }
}
