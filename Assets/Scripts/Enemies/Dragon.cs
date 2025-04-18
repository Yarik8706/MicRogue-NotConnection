using System.Collections;
using Abilities;
using MainScripts;
using PlayersScripts;
using RoomObjects;
using Traps;
using UnityEngine;

namespace Enemies
{
    public class Dragon : TheEnemy, IColdAttack, IFireAttack, IPetrificationAttack, ICompressionAttack
    {
        [Header("Dragon Setting")]
        [SerializeField] private LayerMask noFireLayer;
        [SerializeField] private Transform[] firePosition;
        [SerializeField] private GameObject sleepTimeObject;
        [SerializeField] private Sprite[] sleepTimeSprites;
        [SerializeField] private GameObject fire;
        
        private bool _isFall;
        private bool _isWait;
        private int _waitTime;
        private SpriteRenderer _sleepTimeSpriteRenderer;
        private int _sleepTime;
        private readonly Color _standart = new(255,255,255,1);
        private bool _isSleep;
        private AudioSource _audioSource;
        
        private static readonly int IsSleep = Animator.StringToHash("isSleep");
        
        protected override void Start()
        {
            base.Start();
            _audioSource = GetComponent<AudioSource>();
            animator.SetBool(IsSleep, true);
            _isSleep = true;
            _sleepTimeSpriteRenderer = sleepTimeObject.GetComponent<SpriteRenderer>();
            _sleepTime = sleepTimeSprites.Length;
        }
        
        [ContextMenu("Wake Up")]
        public void WakeUp()
        {
            _isSleep = false;
            animator.SetBool(IsSleep, false);
            StartAnimationTrigger("StartAction");
            Invoke(nameof(TurnOver), 0.5f);
        }

        public override void Active()
        {
            isTurnOver = false;
            switch (_isSleep)
            {
                case false when !_isWait:
                    base.Active();
                    break;
                case true when _sleepTime == 0:
                    WakeUp();
                    return;
                case true:
                    _sleepTime--;
                    _sleepTimeSpriteRenderer.sprite = sleepTimeSprites[_sleepTime];
                    StartCoroutine(DisappearanceAndTurnOver());
                    break;
                default:
                {
                    if(BackroomsController.isBackrooms)
                    {
                        TurnOver();
                        return;
                    };
                    if (_isWait)
                    {
                        if (_waitTime == 2)
                        {
                            StartCoroutine(Fall(
                                GameManager.instance.activeRoomController
                                    .NextPlayerPosition(GameManager.instance.lastExitLocation)));
                        }
                        _waitTime++;
                    }

                    isTurnOver = true;
                    break;
                }
            }
        }

        public override IEnumerator Move(Vector3 @where)
        {
            if(enemyTargetPosition.x > transform.position.x && !turnedRight 
               || enemyTargetPosition.x < transform.position.x && turnedRight)
                Flip();
            return base.Move(@where);
        }

        private IEnumerator DisappearanceAndTurnOver()
        {
            sleepTimeObject.SetActive(true);
            yield return new WaitForSeconds(0.8f);
            var value = 1f;
            while (value > 0)
            {
                value -= Time.deltaTime;
                _sleepTimeSpriteRenderer.color = new Color(255, 255, 255, value);
                yield return null;
            }
            sleepTimeObject.SetActive(false);
            _sleepTimeSpriteRenderer.color = _standart;
            base.TurnOver();
        }

        public override void TurnOver()
        {
            foreach (var spawnFireTransform in firePosition)
            {
                boxCollider2D.enabled = false;
                var hit = Physics2D.Linecast(spawnFireTransform.position, spawnFireTransform.position, noFireLayer);
                boxCollider2D.enabled = true;
                if (hit.collider == null)
                {
                    Instantiate(fire, spawnFireTransform.position, Quaternion.identity);
                }
            }
            base.TurnOver();
        }

        protected override void NextRoomEvent()
        {
            if (_isSleep)
            {
                _isSleep = false;
                animator.SetBool(IsSleep, false);
            }
            _isWait = true;
            _waitTime = 0;
            spriteRenderer.enabled = false;
            boxCollider2D.enabled = false;
        }

        private IEnumerator Fall(Vector3 position)
        {
            _isFall = true;
            transform.position = new Vector3(position.x, position.y + 10, position.z);
            StartCoroutine(SmoothMovement(position));
            yield return new WaitUntil(() => transform.position == position);
            StartCoroutine(CameraShake.instance.Shake(0.5f, 1));
            _audioSource.Play();
            _isFall = false;
            spriteRenderer.enabled = true;
            boxCollider2D.enabled = true;
            TurnOver();
        }

        protected override IEnumerator OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") && _isFall)
            {
                other.gameObject.GetComponent<Player>().Died(this);
                yield break;
            }
            yield return base.OnTriggerEnter2D(other);
        }

        public override void Died() {}

        public void ColdAttack() {}

        public void FireDamage(GameObject firePrefab)
        {
            Instantiate(firePrefab, transform.position, Quaternion.identity);
        }
        
        public void FireDamage(MonoBehaviour killer) {}

        public void Petrification() {}

        public void CompressionDamage() {}
    }
}
