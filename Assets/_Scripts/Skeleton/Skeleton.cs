using UnityEngine;

namespace Tuna
{
    public enum EnemyState
    {
        Chasing,
        Attacking,
        Death,
        Pool
    }

    public class Skeleton : MonoBehaviour
    {
        #region Fields & Properties

        public EnemyState currentState;
        public bool isAlive;

        [SerializeField] private Animator animator;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float attackInterval = 5f;
        [SerializeField] private float attackDuration = 3f;
        [SerializeField] private float attackAngle = 60;
        [SerializeField] private float attackDistance;
        [SerializeField] private SkeletonBullet bullet;
        [SerializeField] private Transform handPosition;
        [SerializeField] private Collider collider;
        [SerializeField] private Rigidbody rigidbody;

        private float _currentAttackDuration;
        private PlayerController target;
        private float lastTimeShooted;
        
        #endregion

        #region Unity Methods

        private void Start()
        {
            target = PlayerController.Instance;
            ChangeState(EnemyState.Chasing);
        }

        private void Update()
        {
            if (isAlive)
            {
                switch (currentState)
                {
                    case EnemyState.Chasing:
                        transform.LookAt(target.transform);
                        transform.Translate(Vector3.forward * (moveSpeed * Time.deltaTime));
                        var targetDist = Vector3.Magnitude(transform.position - target.transform.position);
                        //print(targetDist);
                        if (attackDistance > targetDist)
                        {
                            if (Time.time > lastTimeShooted + attackInterval)
                            {
                                print("Throw");
                                animator.SetTrigger("isAttacking");
                                lastTimeShooted = Time.time;
                                ChangeState(EnemyState.Attacking);
                                _currentAttackDuration = attackDuration;
                            }
                        }
                        break;

                    case EnemyState.Attacking:
                        _currentAttackDuration -= Time.deltaTime;
                        if (_currentAttackDuration < 0)
                        {
                            ChangeState(EnemyState.Chasing);
                        }
                        break;

                    case EnemyState.Death:
                        break;

                    case EnemyState.Pool:
                        break;
                }
            }
        }
        
        #endregion

        #region Private Methods
        
        private void ChangeState(EnemyState newState)
        {
            currentState = newState;
        }

        #endregion

        #region Public Methods

        public void ThrowStoneInstance()
        {
            Vector3 targetDir = target.transform.position - transform.position;
            float angle = Vector3.Angle(targetDir, transform.forward);
            
            
            // Since we began the attack animation,
            // the player may have moved.
            // If the player is still in our sight,
            // we will throw the stone to the corrected position.
            // Otherwise, we will throw it as the animation dictates.
            if (angle < attackAngle)
            {
                var newBullet = Instantiate(bullet, handPosition.position, transform.rotation);
                newBullet.transform.LookAt(target.transform);
                newBullet.transform.eulerAngles = new Vector3(0f, newBullet.transform.eulerAngles.y, 0f);
                Destroy(newBullet.gameObject, 7f);
            }
            else
            {
                var lateEnemyPos = transform.position + transform.forward * attackDistance;
                var newBullet = Instantiate(bullet, handPosition.position, transform.rotation);
                newBullet.transform.LookAt(lateEnemyPos);
                newBullet.transform.eulerAngles = new Vector3(0f, newBullet.transform.eulerAngles.y, 0f);
                Destroy(newBullet.gameObject, 7f);
            }
        }

        public void TakeHit()
        {
            isAlive = false;
            animator.Play("Dying");
            ChangeState(EnemyState.Death);
        }

        public void KillSelf()
        {
            ChangeState(EnemyState.Pool);
            EventManager.onEnemyDeath?.Invoke();
            gameObject.SetActive(false);
        }

        public void Init(Vector3 startingPos)
        {
            rigidbody.velocity = Vector3.zero;
            isAlive = true;
            transform.position = startingPos;
            animator.Play("Run");
            ChangeState(EnemyState.Chasing);
        }
        
        #endregion
    }
}
