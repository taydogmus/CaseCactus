using UnityEngine;

namespace Tuna
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public Vector2 MapBounds;
        
        private void Awake()
        {
            Instance = this;
        }
    }
}