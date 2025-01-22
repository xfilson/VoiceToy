using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// 课程项逻辑脚本;
/// </summary>
[RequireComponent(typeof(PlayableDirector))]
public class CourseLogicTemplate : MonoBehaviour
{
    [BoxGroup("辅助工具")]
    [InfoBox("自动化调整时间轴, 自动纠正音频开始结束发送信号", InfoMessageType.Info)]
    [Button(ButtonSizes.Medium)]
    [VerticalGroup("辅助工具/ButtonGroup")]
    public void MyButton()
    {
        //todo TIMELINE还有复制后的binding问题 https://blog.csdn.net/weixin_45552366/article/details/138133858
        PlayableDirector playableDirector = this.gameObject.GetComponent<PlayableDirector>();
        playableDirector.RebindPlayableGraphOutputs();
    }
}
