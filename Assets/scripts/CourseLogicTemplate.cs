using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefaultNamespace;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// 课程项逻辑脚本;
///
/// https://blog.csdn.net/weixin_39889192/article/details/139257772
/// </summary>
[RequireComponent(typeof(PlayableDirector))]
public class CourseLogicTemplate : MonoBehaviour
{
    [BoxGroup("轨道名称")]
    public string teacher_say_track_name = "老师说话轨道";
    [BoxGroup("轨道名称")]
    public string student_say_track_name  = "学生说话轨道";
    [BoxGroup("轨道名称")]
    public string env_audio_track_name = "环境音轨道";
    [BoxGroup("轨道名称")]
    public string teacher_say_signal_track_name = "老师说话信号轨道";
    [BoxGroup("轨道名称")]
    public string student_say_signal_track_name = "学生说话信号轨道";
    [BoxGroup("发射信号")]
    [BoxGroup("发射信号/老师")]
    public SignalAsset teacher_startSignal;    // 开始信号
    [BoxGroup("发射信号/老师")]
    public SignalAsset teacher_endSignal;      // 结束信号
    [BoxGroup("发射信号/学生")]
    public SignalAsset student_startSignal;    // 开始信号
    [BoxGroup("发射信号/学生")]
    public SignalAsset student_endSignal;      // 结束信号

    private PlayableDirector _playableDirector;
    private void Awake()
    {
        _playableDirector = this.gameObject.GetComponent<PlayableDirector>();
    }

    public void PlayCourse()
    {
        _playableDirector.Play();
    }

#if UNITY_EDITOR
    [BoxGroup("辅助工具")]
    [InfoBox("自动化调整时间轴, 自动纠正音频开始结束发送信号", InfoMessageType.Info)]
    [Button(ButtonSizes.Medium)]
    [VerticalGroup("辅助工具/ButtonGroup")]
    [LabelText("自动调整")]
    public void AutoAdjust()
    {
        var curPlayableDirector = this.gameObject.GetComponent<PlayableDirector>();
        // 获取 Timeline 的所有轨道
        var tracks = (curPlayableDirector.playableAsset as TimelineAsset).GetOutputTracks();

        AudioTrack teacher_say_track = null;
        AudioTrack student_say_track = null;
        SignalTrack teacher_say_signal_track = null;
        SignalTrack student_say_signal_track = null;
        // 检索 Audio 轨道
        foreach (var track in tracks)
        {
            if (track is AudioTrack audioTrack)
            {
                if (audioTrack.name == teacher_say_track_name)
                {
                    teacher_say_track = audioTrack;
                }
                if (audioTrack.name == student_say_track_name)
                {
                    student_say_track = audioTrack;
                }
            }
            if (track is SignalTrack signalTrack)
            {
                if (signalTrack.name == teacher_say_signal_track_name)
                {
                    teacher_say_signal_track = signalTrack;
                }
                if (signalTrack.name == student_say_signal_track_name)
                {
                    student_say_signal_track = signalTrack;
                }
            }
        }

        if (teacher_say_track != null && student_say_track != null && teacher_say_signal_track != null &&
            student_say_signal_track != null)
        {
            ResetSignalTrackByAudioTrack(teacher_say_signal_track, teacher_say_track, teacher_startSignal, teacher_endSignal);
            ResetSignalTrackByAudioTrack(student_say_signal_track, student_say_track, student_startSignal, student_endSignal);
        }
        else
        {
            Debug.LogError("轨道数据不完整");
        }

        forceRefreshTimeline();
    }

    private async void forceRefreshTimeline()
    {
        var preSelectionedObject = Selection.activeGameObject;
        var tmpChildTrans = preSelectionedObject.transform.GetChild(0);
        Selection.activeGameObject = tmpChildTrans.gameObject;
        await Task.Delay(TimeSpan.FromSeconds(0.03f));
        Selection.activeGameObject = preSelectionedObject;
    }

    private void ResetSignalTrackByAudioTrack(SignalTrack signalTrack, AudioTrack audioTrack, SignalAsset startSignal, SignalAsset endSignal)
    {
        //清楚信号轨道原有信号;
        var preMarkers = signalTrack.GetMarkers().ToList();
        foreach (var marker in preMarkers)
        {
            signalTrack.DeleteMarker(marker);
        }
        // 获取 Audio 轨道上的所有剪辑
        var clips = audioTrack.GetClips();
        foreach (var clip in clips)
        {
            var startTime = clip.start;
            var endTime = clip.start + clip.duration;
            
            // 在 Signal 轨道上添加开始信号
            AddSignalEvent(signalTrack, startTime, startSignal);

            // 在 Signal 轨道上添加结束信号
            AddSignalEvent(signalTrack, endTime, endSignal);
        }
    }
    
    
    void AddSignalEvent(SignalTrack signalTrack, double time, SignalAsset signalAsset)
    {
        // 添加到 Signal 轨道
        var courseSignalEmitter = signalTrack.CreateMarker<CourseSignalEmitter>(time);
        courseSignalEmitter.asset = signalAsset;
    }
#endif
}
