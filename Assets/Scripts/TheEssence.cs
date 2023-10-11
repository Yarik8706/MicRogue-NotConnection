using System.Collections;
using System.Collections.Generic;
using MainScripts;
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

public interface IHaveShields
{
	public int GetShieldsCount();
	public void LossShield();
}

public interface IActiveObject {}

public abstract class TheEssence : MonoBehaviour, IActiveObject
{
	[Header("Movement Parameters")]
	public LayerMask blockingLayer;
	public Vector2[] variantsPositions;
	[Header("Animations Parameters")] 
    public string moveAnimationName;
    public string diedAnimationName;
    [Header("Other")] 
    [SerializeField] private protected GameObject diedEffect;
    [SerializeField] private protected AudioClip diedMusic;
    
    public bool turnedRight { get; protected set; }
    public Vector3 movingPosition { get; protected set; }
    
    private protected BoxCollider2D boxCollider2D;
    private protected float inverseMoveTime;
    private protected AnimationType diedAnimation;
    private protected AnimationType moveAnimation;
    private protected float moveTime = .1f;
    private Rigidbody2D _rigidbody2d;

    internal SpriteRenderer spriteRenderer;
    internal Animator animator;
    internal TheEssenceEffect essenceEffect = TheEssenceEffect.None;
    internal bool isTurnOver = true;
    internal bool isMove;
    internal bool isActive = true;

    protected virtual void Start() {}
    
    public virtual void Awake()
    {
	    moveAnimation = new AnimationType(moveAnimationName);
	    diedAnimation = new AnimationType(diedAnimationName);
	    turnedRight = transform.localScale.x < 0;
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

    protected void Flip()
    {
        Flip(!turnedRight);
    }
    
    public virtual void Flip(bool turnedRight)
    {
	    this.turnedRight = turnedRight;
	    var scaler = transform.localScale;
	    scaler.x = turnedRight ? -1 : 1;
	    transform.localScale = scaler;
    }
    
    public virtual IEnumerator Move(Vector3 @where)
    {
	    movingPosition = where;
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
	    _rigidbody2d.MovePosition(end);
		isMove = false;
    }
    
    public static Vector2[] VariantsPositionsNow(Vector2 centerPosition, Vector2[] positions)
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
    
    protected virtual IEnumerator AttackPlayer(TheEssence attackEssence)
    {
	    if(attackEssence.transform.position.x < transform.position.x && turnedRight 
	       || attackEssence.transform.position.x > transform.position.x && !turnedRight)
	    {
		    Flip();
	    }
	    boxCollider2D.enabled = false;
	    var enemyPosition = transform.position;
	    if (!CheckEssencesShields(attackEssence))
	    {
		    StartCoroutine(Move(attackEssence.transform.position));
		    yield break;
	    } 
	    var attackVector = (Vector2)(transform.position - attackEssence.transform.position).normalized;
	    var attackPosition = (Vector2)attackEssence.transform.position + attackVector * 0.6f;
	    Vector2 endPosition;

	    if ((transform.position - attackEssence.transform.position).sqrMagnitude <= 2f)
	    {
		    endPosition = enemyPosition;
	    }
	    else
	    {
		    endPosition = new Vector2((attackEssence.transform.position.x + enemyPosition.x) / 2, 
			    (attackEssence.transform.position.y + enemyPosition.y) / 2);
	    }
            
	    StartCoroutine(SmoothMovement(attackPosition));
	    yield return new WaitUntil(() => !isMove);
	    (attackEssence as IHaveShields).LossShield();
	    StartCoroutine(SmoothMovement(endPosition));
	    yield return new WaitUntil(() => !isMove);
	    yield return new WaitForSeconds(0.3f);
	    TurnOver();
    }
    
    protected virtual IEnumerator OnTriggerEnter2D(Collider2D other)
    {
	    if (!isTurnOver) yield break;
	    if (!other.gameObject.TryGetComponent(out TheEssence essence)) yield break;
	    if (isMove || !essence.isMove) yield break;
	    yield return new WaitUntil(() => essence.isMove == false);
	    if (essence.movingPosition != transform.position) yield break;
	    Died(essence);
    }
    
    protected bool CheckEssencesShields(TheEssence essence)
    {
	    var hit = Physics2D.Linecast(transform.position, essence.transform.position, GameController.instance.shieldLayer);
	    boxCollider2D.enabled = true;
	    return hit.collider != null && hit.collider.transform.parent == essence.transform 
	           && (essence as IHaveShields).GetShieldsCount() != 0;
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
    }

    public virtual void TurnOver()
    {
	    isMove = false;
	    isTurnOver = true;
    }

    public virtual void DiedWithMusic()
    {
	    StartAnimationTrigger(diedAnimation);
	    var effect = Instantiate(diedEffect, transform.position, Quaternion.identity);
	    effect.GetComponent<AudioSource>().clip = diedMusic;
	    TurnOver();
	    Destroy(gameObject);
    }
    
    public virtual void Died()
    {
	    StartAnimationTrigger(diedAnimation);
	    Instantiate(diedEffect, transform.position, Quaternion.identity);
	    TurnOver();
	    Destroy(gameObject);
    }
    
    public virtual void Died(MonoBehaviour killer)
    {
	    Died();
    }
}
