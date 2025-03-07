using Spine.Unity;
using UnityEngine;

namespace DefaultNamespace
{
    public class SpineTest : MonoBehaviour
    {
        public SkeletonAnimation skeletonAnimation;

        public void GoIdle()
        {
            skeletonAnimation.AnimationState.AddAnimation(0, "idle", true, 0.2f); // 0.2秒过渡时间
        }

        public void GoRun()
        {
            skeletonAnimation.AnimationState.AddAnimation(0, "run", true, 0.2f); // 0.2秒过渡时间
        }
    }
}