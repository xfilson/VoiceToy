using UnityEngine;

public class LoopAnimatorState : MonoBehaviour
{
    public Animator animator;
    public string defaultStateName = "idle";
    public string stateName; // 动画状态名称
    public int loopCount = 3; // 指定循环次数
    private int currentLoop = 0;

    private bool isNeedLoopCheck = false;
    public void SetLoopCheck(bool value, Animator animator = null, string stateName = "", int loopCount = -1, string defaultStateName = "idle")
    {
        this.isNeedLoopCheck = value;
        this.animator = animator;
        this.stateName = stateName;
        this.loopCount = loopCount;
        this.defaultStateName = defaultStateName;
    }

    void Update()
    {
        if (!isNeedLoopCheck)
        {
            return;
        }

        // 获取当前状态信息
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // 检查是否是目标状态且是否播放完毕
        if (stateInfo.IsName(stateName) && stateInfo.normalizedTime >= 1f)
        {
            currentLoop++;

            // 如果未达到指定循环次数，重新播放状态
            if (currentLoop < loopCount || loopCount <= 0)
            {
                animator.Play(stateName, 0, 0f);
            }
            else
            {
                // 达到指定循环次数后，重置循环计数
                currentLoop = 0;
                this.isNeedLoopCheck = false;
                animator.Play(defaultStateName);
            }
        }
    }
}