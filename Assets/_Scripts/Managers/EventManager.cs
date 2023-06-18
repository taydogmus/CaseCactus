using System;

namespace Tuna
{
    public static class EventManager
    {
        public static Action onCoinUpdated;
        public static Action onEnemyDeath;
        public static Action onPlayerDeath;
        public static Action<float> onPlayerTakeDamage;
    }
}