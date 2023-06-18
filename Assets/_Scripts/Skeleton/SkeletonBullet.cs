using UnityEngine;

namespace Tuna
{
    public class SkeletonBullet : MonoBehaviour
    {
        [SerializeField] private float bulletSpeed;
        private void Update()
        {
            transform.position += transform.forward * (bulletSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                var player = other.gameObject.GetComponent<PlayerController>();
                player.TakeHit();
                Destroy(gameObject);
            }
        }
    }
}