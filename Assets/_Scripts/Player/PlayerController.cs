using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Tuna
{
    public class PlayerController : MonoBehaviour
    {
        #region Fields & Properties
        public static PlayerController Instance;
        public Canvas canvas;
        
        [SerializeField] private float sensitivity;
        [SerializeField] private Joystick joystick;
        [SerializeField] private Rigidbody rigidbody;
        [SerializeField] private Animator animator;
        [SerializeField] private Boomerang boomerang;
        [SerializeField] private float radius = 5.0f;
        [SerializeField] private float startingHealth = 7;
        [SerializeField] private float attackInterval = 1.1f;
        [SerializeField] private SpriteRenderer rangeSprite;
        [SerializeField] private Color mainSpriteColor;

        private Skeleton closestSkeleton = null;
        private Vector3 _moveDirection;
        private Quaternion _lastRotation;
        private float _currentHealth;
        private bool _isAlive = true;
        private Skeleton _closestEnemy;
        private float lastAttackTime;
        private float _health
        {
            get => _currentHealth;
            set
            {
                _currentHealth = value;
                var healthPercentage = (_health / startingHealth);
                EventManager.onPlayerTakeDamage?.Invoke(healthPercentage);
                isAlive = _currentHealth > 0 ? true : false;
            }
        }
        
        public bool isAlive
        {
            get { return _isAlive; }
            set
            {
                if (value == false)
                {
                    animator.SetTrigger("isDead");
                    EventManager.onPlayerDeath?.Invoke();
                }
                _isAlive = value;
            }
        }
        
        #endregion

        #region UnityMethods

        private void Awake()
        {
            Instance = this;
            _health = startingHealth;
        }

        private void Start()
        {
            StartCoroutine(UpdateTargetCoroutine());
        }

        private void Update()
        {
            if (isAlive)
            {
                var inputVector = new Vector3(joystick.Horizontal, 0f, joystick.Vertical);
                _moveDirection = Vector3.ClampMagnitude(inputVector, 1);    
            }
            else
            {
                _moveDirection = Vector3.zero;
            }
            
        }
        
        private void FixedUpdate()
        {
            Movement();
            TryTrowBoomerang();
            
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                if(!isAlive)
                    return;
                var enemy = collision.gameObject.GetComponent<Skeleton>();
                if (enemy.isAlive)
                {
                    CameraShake.Instance.ShakeCamera(2,.5f);
                    _health = 0;
                }
            }
        }
        
        #endregion

        #region Private Methods
        
        private void TryTrowBoomerang()
        {
            //Throw boomerang if conditions are met
            if (isAlive && closestSkeleton != null)
            {
                if (Time.time > lastAttackTime + attackInterval)
                {
                    var newBoomerang = Instantiate(boomerang, transform.position, Quaternion.identity);
                    newBoomerang.Init(closestSkeleton);
                    lastAttackTime = Time.time;
                    
                    // Lets add some visual effects because why not?
                    rangeSprite.color = Color.red;
                    rangeSprite.DOColor(mainSpriteColor, .25f).SetEase(Ease.InCubic);
                }

                closestSkeleton = null;
            }
        }

        private void Movement()
        {            
            if (_moveDirection.sqrMagnitude > 0.01)
            {
                transform.position += _moveDirection * (sensitivity * Time.fixedDeltaTime);
                _lastRotation = Quaternion.LookRotation(_moveDirection);
                transform.rotation = _lastRotation;
                animator.SetBool("isWalking", true);
            }
            else
            {
                transform.rotation = _lastRotation;
                animator.SetBool("isWalking", false);
            }
            
            //Apply gravity in FixedUpdate for detecting collisions
            rigidbody.AddForce(Physics.gravity * rigidbody.mass);
        }
        
        private IEnumerator UpdateTargetCoroutine()
        {
            while (isAlive)
            {
                // Detect all colliders within a sphere with the given radius
                Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

                closestSkeleton = null;
                float closestDistance = float.MaxValue;

                // Loop through all detected colliders
                foreach (Collider collider in colliders)
                {
                    // Check if the collider's game object has a Skeleton component
                    // TODO: Convert to tryGetComponent
                    Skeleton skeleton = collider.gameObject.GetComponent<Skeleton>();
                    if (skeleton != null && skeleton.isAlive)
                    {
                        // Calculate the distance to the skeleton
                        float distance = Vector3.Distance(transform.position, skeleton.transform.position);

                        // Check if this is the closest skeleton so far
                        if (distance < closestDistance)
                        {
                            closestSkeleton = skeleton;
                            closestDistance = distance;
                        }
                    }
                }
                
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
        
        #endregion

        #region Public Methods

        public void TakeHit()
        {
            if(!isAlive)
                return;
            CameraShake.Instance.ShakeCamera(1,.5f);
            _health--;
        }

        #endregion

        #region Comments

            // I found that my player is always at the middle of the screen
            // So I don't need to adjust the position of the HealthBar
            // I'm not calling it anymore, but here is how I would do it:
            //private void AdjustHealthBarPosition()
            //{         
            //    //Conversion of world position to canvas position
            //    Vector2 canvasPosition;
            //    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
            //        Camera.main.WorldToScreenPoint(transform.position),
            //        canvas.worldCamera,
            //        out canvasPosition);
            //    var positionOffset = canvas.pixelRect.height * 0.06; 
            //    canvasPosition.y += (float)positionOffset;
            //}

        #endregion
    }
}