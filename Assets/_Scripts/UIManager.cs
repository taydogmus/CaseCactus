using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Tuna
{
    public class UIManager : MonoBehaviour
    {
        #region Fields
        
        public static UIManager Instance;

        [SerializeField] private TextMeshProUGUI skullText;
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private Transform coinIcon;
        [SerializeField] private Transform skullIcon;
        [SerializeField] private Image healthBar;
        [SerializeField] private GameObject retryMenu;
        
        public Canvas Canvas;
        public Image HealthBarBG;

        private int enemiesKilled = 0;
        
        #endregion
        
        #region UnityMethods
        private void Awake()
        {
            Instance = this;
            
            EventManager.onCoinUpdated += RefreshCoinCount;
            EventManager.onEnemyDeath += RegisterDeathEnemy;
            EventManager.onPlayerDeath += ShowGameOverUI;
            EventManager.onPlayerTakeDamage += SetHealthBarRatio;
        }

        private void Start()
        {
            RefreshCoinCount();
        }

        private void OnDestroy()
        {
            EventManager.onCoinUpdated -= RefreshCoinCount;
            EventManager.onEnemyDeath -= RegisterDeathEnemy;
            EventManager.onPlayerDeath -= ShowGameOverUI;
            EventManager.onPlayerTakeDamage -= SetHealthBarRatio;
        }

        public void RestartScene()
        {
            SceneManager.LoadScene(0);
        }

        #endregion
        
        #region Private Methods
        
        private void RefreshCoinCount()
        {
            coinIcon.DOPunchScale(Vector3.one * .2f, .1f, 3, 1f).OnComplete(() =>
            {
                coinIcon.transform.localScale = Vector3.one;
            });
            coinText.text = Registry.CoinCount.ToString();
        }
        
        private void RegisterDeathEnemy()
        {
            skullIcon.DOPunchScale(Vector3.one * .2f, .1f, 3, 1f).OnComplete(() =>
            {
                coinIcon.transform.localScale = Vector3.one;
            });
            enemiesKilled++;
            skullText.text = enemiesKilled.ToString();
        }
        
        private void SetHealthBarRatio(float ratio)
        {
            healthBar.DOKill(true);
            healthBar.DOFillAmount(ratio, .15f);
        }

        private void ShowGameOverUI()
        {
            retryMenu.SetActive(true);
        }
        
        #endregion
    }
}