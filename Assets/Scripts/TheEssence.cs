using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

public struct AnimationType
{
	public readonly string name;
	public float speed;

	public AnimationType(string name = "", float speed = 1)
	{
		this.name = name;
		this.speed = speed;
	}
}

public interface IActiveObject {}

public interface ICauseOfDied
{
	public string GetDeathText();
}

public abstract class TheEssence : MonoBehaviour, IActiveObject
{
	[Header("Movement Parameters")]
	public LayerMask blockingLayer;
	public Vector2[] variantsPositions;
	[Header("Animations Parameters")] 
    public string moveAnimationName;
    public string diedAnimationName;
    [Header("Other")] 
    public GameObject baseAnimationsObj;
    
    private protected bool turnedRight;
    private protected BoxCollider2D boxCollider2D;
    private protected float inverseMoveTime;
    private protected AnimationType diedAnimation;
    private protected AnimationType moveAnimation;
    private protected float moveTime = .1f;
    private Rigidbody2D _rigidbody2d;

    internal SpriteRenderer spriteRenderer;
    internal Animator animator;
    internal TheEssenceEffect essenceEffect = TheEssenceEffect.None;
    internal bool isTurnOver;
    internal bool isMove;
    internal bool isActive;

    protected virtual void Start() {}
    
    public virtual void Awake()
    {
	    isActive = true;
	    moveAnimation = new AnimationType(moveAnimationName);
	    diedAnimation = new AnimationType(diedAnimationName);
	    turnedRight = !(transform.localScale.x > 0);
	    inverseMoveTime = 1f / moveTime;
	    spriteRenderer = GetComponent<SpriteRenderer>();
	    animator = GetComponent<Animator>();
	    boxCollider2D = GetComponent<BoxCollider2D>();
	    _rigidbody2d = GetComponent<Rigidbody2D>();
    }

    public virtual void Active()
    {
	    isTurnOver = false;
    }

    protected virtual Vector2[] MoveCalculation(Vector2[] theVariantsPositions)
    {
	    return PositionCalculation(transform.position, theVariantsPositions, blockingLayer, boxCollider2D);
    }

    protected void SetAnimationMoveSpeed(float animationSpeed, float moveTimeSpeed)
    {
	    moveAnimation.speed = animationSpeed;
	    moveTime = moveTimeSpeed;
	    inverseMoveTime = 1f / moveTime;
    }

    protected void Flip()
    {
        turnedRight = !turnedRight;
        var scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
    
    protected void Flip(bool turnedRight)
    {
	    this.turnedRight = turnedRight;
	    var scaler = transform.localScale;
	    scaler.x = turnedRight ? 1 : -1;
	    transform.localScale = scaler;
    }
    
    public virtual IEnumerator Move(Vector3 @where)
    {
	    StartAnimationTrigger(moveAnimation);
	    yield return SmoothMovement(@where);
	    TurnOver();
    }
    
    protected IEnumerator SmoothMovement(Vector3 end)
    {
	    isMove = true;
	    yield return new WaitForSeconds(0.1f);
	    while(transform.position != end)
	    {
		    var newPosition = Vector3.MoveTowards(
			    _rigidbody2d.position, 
			    end, 
			    inverseMoveTime * Time.deltaTime);
		    transform.position = newPosition;
		    yield return null;
	    }
		// // _rigidbody2d.MovePosition(end);
		isMove = false;
    }
    
    protected static Vector2[] VariantsPositionsNow(Vector2 centerPosition, Vector2[] positions)
    {
	    var newVariantsPositions = new Vector2[positions.Length];
	    for (int i = 0; i < positions.Length; i++)
	    {
		    newVariantsPositions[i] = positions[i] + centerPosition;
	    }
        return newVariantsPositions;
    }
    
    protected Vector2[] VariantsPositionsNow(Vector2[] positions)
    {
	    var newVariantsPositions = new Vector2[positions.Length];
	    for(int i = 0; i < positions.Length; i++)
	    {
		    newVariantsPositions[i] = positions[i] + (Vector2)transform.position;
	    }
	    return newVariantsPositions;
    }

    protected void StartAnimationTrigger(AnimationType animationType)
    {
	    if (animationType.name == "")
	    {
		    return;
	    }
	    animator.speed = animationType.speed;
	    animator.SetTrigger(animationType.name);
    }
    
    protected void StartAnimationTrigger(string trigger)
    {
	    if (trigger == "")
	    {
		    return;
	    }
	    animator.speed = 1;
	    animator.SetTrigger(trigger);
    }
    
    public void StartAnimation(string nameAnimation)
    {
	    if (nameAnimation == "")
	    {
		    return;
	    }
	    animator.speed = 1;
	    animator.Play(nameAnimation);
    }

    public static Vector2[] PositionCalculation(Vector2 startPosition, Vector2[] calculationPositions, LayerMask blockingLayer, BoxCollider2D boxCollider2D)
    {
	    var nowVariantsPositions = new List<Vector2> {};
	    boxCollider2D.enabled = false;
	    foreach (var newVariantPosition in calculationPositions)
	    {
		    var hit = Physics2D.Linecast(startPosition, newVariantPosition, blockingLayer);
		    if (hit.collider == null)
		    {
			    nowVariantsPositions.Add(newVariantPosition);
		    }
	    }
	    boxCollider2D.enabled = true;
	    return nowVariantsPositions.ToArray();
	    // var nowVariantsPositions = new List<Vector2> {Capacity = 0};
	    // boxCollider2D.enabled = false;
	    // nowVariantsPositions.AddRange(
		   //  from newVariantPosition 
			  //   in calculationPositions let hit = Physics2D.Linecast(startPosition, newVariantPosition, blockingLayer) where hit.collider == null select newVariantPosition);
	    // boxCollider2D.enabled = true;
	    // return nowVariantsPositions.ToArray();
    }

    protected virtual void TurnOver()
    {
	    isMove = false;
	    isTurnOver = true;
    }
    
    public virtual void Died()
    {
	    StartAnimationTrigger(diedAnimation);
	    Instantiate(baseAnimationsObj, transform.position, Quaternion.identity)
		    .GetComponent<BaseAnimations>().DiedAnimation();
	    TurnOver();
	    Destroy(gameObject);
    }
    
    public virtual void Died(MonoBehaviour killer)
    {
	    Died();
    }
}
