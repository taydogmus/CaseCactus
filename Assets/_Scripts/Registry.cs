using UnityEngine;

namespace Tuna
{
    //Static class for managing coin count and related actions
    public static class Registry
    {
        private const string CoinCountKey = "CoinCountKey";

        public static int CoinCount
        {
            set => PlayerPrefs.SetInt(CoinCountKey, value);
            get => PlayerPrefs.GetInt(CoinCountKey, 0);
        }
        
        public static void UpdateCoins(int coins)
        {
            CoinCount += coins;
            PlayerPrefs.Save();
            EventManager.onCoinUpdated?.Invoke();
        }
    }
}