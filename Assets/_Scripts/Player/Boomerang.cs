using UnityEngine;

namespace Tuna
{
    public class Boomerang : MonoBehaviour
    {
        #region Properties & Fields
        
        [SerializeField] private float speed = 0.1F;
        [SerializeField] private float maxOffset = 1.0F;
        [SerializeField] private float spinSpeed = 30f;
        [SerializeField] private Transform model;
        
        private Skeleton _skeleton;
        private float startTime;
        private float journeyLength;
        private Vector3 startPosition;
        private Vector3 endPosition;
        
        #endregion

        #region Unity Methods
        
        private void Update()
        {
            
            // Lerp from Unity's documentation.
            // Distance moved equals elapsed time times speed..
            float distCovered = (Time.time - startTime) * speed;

            // Fraction of journey completed equals current distance divided by total distance.
            float fractionOfJourney = distCovered / journeyLength;

            // Calculate an offset to mimic boomerang movement
            // Offset is maximized at the middle of the journey
            float offset = maxOffset * Mathf.Sin(Mathf.PI * fractionOfJourney);

            // Set our position as a fraction of the distance between the markers with an offset.
            Vector3 newPosition = Vector3.Lerp(startPosition, endPosition, fractionOfJourney);
            newPosition += offset * transform.right;
            transform.position = newPosition;
            
            //SpinModel
            model.Rotate(0f, 0f, spinSpeed);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                var enemy = other.gameObject.TryGetComponent<Skeleton>(out Skeleton enemySkeleton);
                Destroy(gameObject);
                enemySkeleton.TakeHit();
            }    
        }
        
        #endregion
        
        #region Public Methods
        
        public void Init(Skeleton skeleton)
        {
            _skeleton = skeleton;
            transform.LookAt(skeleton.transform);
            startTime = Time.time;

            var endPosCorrected = _skeleton.transform.position;
            endPosCorrected.y = 1.5f;
            endPosition = endPosCorrected;
            
            // Calculate the journey length.
            journeyLength = Vector3.Distance(transform.position, endPosition);

            var startPosCorrected = transform.position;
            startPosCorrected.y = 1.5f;
            startPosition = startPosCorrected;
            Destroy(gameObject, 3f);
        }

        #endregion
    }
}
