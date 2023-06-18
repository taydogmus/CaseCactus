using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

namespace Tuna
{
    
    //Actually this is old way to create a pool
    //Unity already supports pooling but i have limited time :)
    public class SkeletonPoolManager : MonoBehaviour
    {
        #region Fields && Properties
        
        public static SkeletonPoolManager Instance;
        [SerializeField] private int initialSkeletonCount;
        [SerializeField] private int minAliveEnemy;
        [SerializeField] private Skeleton skeleton;
        //Since we serialized the list, we dont need to instantiate it
        [SerializeField] private List<Skeleton> skeletonPool;
        [SerializeField] private Slider slider;
        
        private PlayerController player;
        private int enemiesKilled;
        private int difficultyAdd = 0;
        private float difficulty = 0;
        
        #endregion

        #region Unity Methods
        
        private void Awake()
        {
            Instance = this;

            EventManager.onEnemyDeath += AdjustDifficulty;

            for (int i = 0; i < initialSkeletonCount; i++)
            {
                AddNewSkeletonToPool();
            }
        }
        
        private void Start()
        {
            player = PlayerController.Instance;
        }

        private void Update()
        {
            int aliveEnemyCount = CountAliveEnemies();
            if (aliveEnemyCount < minAliveEnemy + difficultyAdd)
            {
                Skeleton newEnemy = TryGetSkeletonFromPool();
                newEnemy.gameObject.SetActive(true);
                newEnemy.Init(RandomPosition());
            }
        }
        
        private void OnDestroy()
        {
            EventManager.onEnemyDeath -= AdjustDifficulty;
            skeletonPool.Clear();
        }

        #endregion

        #region Private Methods
        
        private int CountAliveEnemies()
        {
            return skeletonPool.Count(x =>
                x.currentState == EnemyState.Chasing ||
                x.currentState == EnemyState.Attacking);
        }

        private Skeleton TryGetSkeletonFromPool()
        {
            var deActiveSkeletons = skeletonPool.Where(x => x.currentState == EnemyState.Pool).ToList();
            if (deActiveSkeletons.Count > 0)
            {
                return deActiveSkeletons[0];
            }
            return AddNewSkeletonToPool();
        }

        private Skeleton AddNewSkeletonToPool()
        {
            var newSkeleton = Instantiate(skeleton, transform);
            newSkeleton.gameObject.SetActive(false);
            skeletonPool.Add(newSkeleton);
            return newSkeleton;
        }

        private Vector3 RandomPosition()
        {
            Vector2 mapBounds = GameManager.Instance.MapBounds;
            var boundX = mapBounds.x;
            var boundY = mapBounds.y;

            float randomX, randomZ;
            bool withinBounds = false;
            while (!withinBounds)
            {
                randomX = Random.Range(-29, 29) * (Random.value < 0.5f ? -1f : 1f);
                randomZ = Mathf.Abs(randomX) > 14 ? Random.Range(-15f, 15f) : Random.Range(30f, 35f) * (Random.value < 0.5f ? -1f : 1f);
                Vector3 randomPosition = new Vector3(player.transform.position.x + randomX, 1.5f, player.transform.position.z + randomZ);
                withinBounds = randomPosition.x >= -boundX && randomPosition.x <= boundX && randomPosition.z >= -boundY && randomPosition.z <= boundY;
                if (withinBounds)
                {
                    return randomPosition;
                }
            }
            //This point is never reached since we can find a position withing mapBounds
            return Vector3.zero;
        }
        
        private void AdjustDifficulty()
        {
            enemiesKilled++;

            if (enemiesKilled % 2 == 0)
            {
                difficultyAdd++;
                
                if(difficulty > 10)
                    return;
                difficulty = difficultyAdd / 10f;
                slider.value = difficulty;
            }
        }
        
        #endregion
    }
}
