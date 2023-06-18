using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tuna
{
    //Actually this is old way to create a pool
    //Unity already supports pooling but i have limited time :)
    public class CollectablePoolManager : MonoBehaviour
    {
        #region Properties & Fields
        
        public static CollectablePoolManager Instance;
        
        [SerializeField] private int initialPoolSize;
        [SerializeField] private int minCoin;
        [SerializeField] private int maxVisibleCoins = 17;

        [SerializeField] private Collectable skeleton;
        //Since we serialized the list above, its already instantiated
        [SerializeField] private List<Collectable> collectablePool;

        private  PlayerController player;
        
        #endregion

        #region UnityMethods

        private void Awake()
        {
            Instance = this;
            
            collectablePool = new List<Collectable>();

            //Create a pool
            for (int i = 0; i < initialPoolSize; i++)
            {
                AddNewSkeletonToPool();
            }
        }

        private void Start()
        {
            player = PlayerController.Instance;

            // We need some visible coins at the start 
            for (int i = 0; i < maxVisibleCoins; i++)
            {
                Collectable newCoin = TryGetCoinFromPool();
                newCoin.gameObject.SetActive(true);
                var closePosition = Random.insideUnitSphere * 15;
                closePosition.y = 1.5f;
                newCoin.Init(closePosition);
            }
        }
        
        private void Update()
        {
            int aliceCoinCount = CountCoinsRemaining();
            if (aliceCoinCount < Mathf.Max(minCoin, maxVisibleCoins))
            {
                Collectable newCoin = TryGetCoinFromPool();
                newCoin.gameObject.SetActive(true);
                newCoin.Init(RandomPosition());
            }
        }
        
        private void OnDestroy()
        {
            collectablePool.Clear();
        }
        
        #endregion

        #region PrivateMethods

        
        private int CountCoinsRemaining()
        {
            //Linq is quiet cheap to use and what we do here is:
            //Finding active coinCount
            return collectablePool.Count(x =>
                x.gameObject.activeInHierarchy);
        }

        private Collectable TryGetCoinFromPool()
        {
            var deActiveSkeletons = collectablePool.Where(x => !x.gameObject.activeInHierarchy).ToList();
            if (deActiveSkeletons.Count > 0)
            {
                return deActiveSkeletons.FirstOrDefault();
            }
            return AddNewSkeletonToPool();
        }

        private Collectable AddNewSkeletonToPool()
        {
            var newSkeleton = Instantiate(skeleton, transform);
            newSkeleton.gameObject.SetActive(false);
            collectablePool.Add(newSkeleton);
            return newSkeleton;
        }

        private Vector3 RandomPosition()
        {
            Vector2 mapBounds = GameManager.Instance.MapBounds;
            var boundX = mapBounds.x;
            var boundY = mapBounds.y;

            float randomX, randomZ;
            var withinBounds = false;
            while (!withinBounds)
            {
                randomX = Random.Range(-29, 29) * (Random.value < 0.5f ? -1f : 1f);
                randomZ = Mathf.Abs(randomX) > 14 ? Random.Range(-15f, 15f) : Random.Range(30f, 35f) * (Random.value < 0.5f ? -1f : 1f);
                var randomPosition = new Vector3(player.transform.position.x + randomX, 1.5f, player.transform.position.z + randomZ);
                withinBounds = randomPosition.x >= -boundX && randomPosition.x <= boundX && randomPosition.z >= -boundY && randomPosition.z <= boundY;
                if (withinBounds)
                {
                    return randomPosition;
                }
            }
            //This point is never reached since we can find a position withing mapBounds
            return Vector3.zero;
        }
        
        #endregion
    }
}
