using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
	public bool CheckShieldActive();
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
    private protected AnimationType diedAnimation;
    private protected AnimationType moveAnimation;
    
    internal SpriteRenderer spriteRenderer;
    internal Animator animator;
    internal TheEssenceEffect essenceEffect = TheEssenceEffect.None;
    internal bool isTurnOver = true;
    internal bool isMove;
    internal bool isActive = true;
    
    private float _moveTime = .22f;
	private float _meleeMoveTime = .15f;

    protected virtual void Start() {}
    
    public virtual void Awake()
    {
	    moveAnimation = new AnimationType(moveAnimationName);
	    diedAnimation = new AnimationType(diedAnimationName);
	    turnedRight = transform.localScale.x < 0;
	    spriteRenderer = GetComponent<SpriteRenderer>();
	    animator = GetComponent<Animator>();
	    boxCollider2D = GetComponent<BoxCollider2D>();
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
	    var moveTimeNow = _moveTime;
	    if (Vector2.Distance(end, transform.position) <= 1) moveTimeNow = _meleeMoveTime;
	    transform.DOMove(end, moveTimeNow).SetEase(Ease.OutCubic).SetLink(gameObject);
	    yield return new WaitForSeconds(moveTimeNow + 0.2f);
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
	    if((int)attackEssence.transform.position.x < (int)transform.position.x && turnedRight 
	       || (int)attackEssence.transform.position.x > (int)transform.position.x && !turnedRight)
	    {
		    Flip();
	    }
	    var enemyPosition = transform.position;
	    if (attackEssence is not IHaveShields shieldData
	        || CanKillEssence(attackEssence, transform.position))
	    {
		    StartCoroutine(Move(attackEssence.transform.position));
		    yield break;
	    } 
	    
	    var attackVector = (Vector2)(transform.position - attackEssence.transform.position).normalized;
	    var attackPosition = (Vector2)attackEssence.transform.position + attackVector * 0.6f;
	    Vector2 endPosition;

	    if (Vector2.Distance(enemyPosition, attackEssence.transform.position) < 2f)
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
	    shieldData.LossShield();
	    StartCoroutine(SmoothMovement(endPosition));
	    yield return new WaitUntil(() => !isMove);
	    yield return new WaitForSeconds(0.3f);
	    TurnOver();
    }
    
    protected virtual IEnumerator OnTriggerEnter2D(Collider2D other)
    {
	    if (!isTurnOver) yield break;
	    if (!other.gameObject.TryGetComponent(out TheEssence essence)) yield break;
	    yield return new WaitUntil(() => essence.isTurnOver);
	    if (essence.movingPosition != transform.position) yield break;
	    Died(essence);
    }

    protected static bool CanKillEssence(TheEssence essence, Vector3 attackerPosition)
    {
	    if (essence is not IHaveShields haveShields) return false;
	    var hit = Physics2D.Linecast(attackerPosition, essence.transform.position, 
		    GameController.instance.shieldLayer);
	    return !(hit.collider != null 
	           && hit.collider.transform.parent == essence.transform 
	           && haveShields.GetShieldsCount() != 0)
	           && !haveShields.CheckShieldActive();
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
