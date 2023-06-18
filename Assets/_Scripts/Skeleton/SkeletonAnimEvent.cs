using UnityEngine;

namespace Tuna
{
    public class SkeletonAnimEvent : MonoBehaviour
    {

        [SerializeField] private Skeleton skeleton;
        
        public void ThrowStone()
        {
            skeleton.ThrowStoneInstance();
        }
        
        public void DeactivateSkeleton()
        {
            skeleton.KillSelf();
        }
    }
}