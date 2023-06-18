using UnityEngine;
using DG.Tweening;

namespace Tuna
{
    public class Collectable : MonoBehaviour
    {
        private void Start()
        {
            StartMinglingAround();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Registry.UpdateCoins(1);
                transform.DOKill();
                gameObject.SetActive(false);
            }
        }

        private void StartMinglingAround()
        {
            transform.DOKill();
            transform.DOMoveY(0.5f, 1f).SetLoops(-1, LoopType.Yoyo);
            transform.DORotate(new Vector3(0f, 180, 0), 4f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }

        public void Init(Vector3 _pos)
        {
            transform.position = _pos;
            StartMinglingAround();
        }
    }
}