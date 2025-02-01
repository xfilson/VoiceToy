using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefaultNamespace;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using System.IO;
using FriendlyMonster.RhubarbTimeline;
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// 课程项逻辑脚本;
///
/// Timelne Api
/// https://blog.csdn.net/weixin_39889192/article/details/139257772
///
/// lipsync 嘴型
/// https://github.com/DanielSWolf/rhubarb-lip-sync
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
    [BoxGroup("轨道名称")]
    public string teacher_say_lipsync_track_name = "老师嘴型轨道";
    [BoxGroup("轨道名称")]
    public string student_say_lipsync_track_name = "学生嘴型轨道";
    [BoxGroup("发射信号")]
    [BoxGroup("发射信号/老师")]
    public SignalAsset teacher_startSignal;    // 开始信号
    [BoxGroup("发射信号/老师")]
    public SignalAsset teacher_endSignal;      // 结束信号
    [BoxGroup("发射信号/学生")]
    public SignalAsset student_startSignal;    // 开始信号
    [BoxGroup("发射信号/学生")]
    public SignalAsset student_endSignal;      // 结束信号
    
    [BoxGroup("发射信号/嘴型音频振幅")]
    public SignalAsset lipAudioAmplitudeSignal;
    [BoxGroup("发射信号/嘴型音频振幅")]
    //嘴型数据振幅和上一次振幅的所需偏移值;
    public float lipAmplitudeOffset = 0.0f;
    [BoxGroup("发射信号/嘴型音频振幅")]
    //嘴型数据振幅和上一次的间隔;
    public float lipAmplitudeInterval = 0.12f;
    [BoxGroup("发射信号/嘴型音频振幅")]
    //嘴型数据最小值;
    public float lipAmplitudeIgnore = 0.01f;
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

        RhubarbPlayableTrack teacher_lipsync_track = null;
        RhubarbPlayableTrack student_lipsync_track = null;
        AudioTrack teacher_say_track = null;
        AudioTrack student_say_track = null;
        SignalTrack teacher_say_signal_track = null;
        SignalTrack student_say_signal_track = null;
        // 检索 Audio 轨道
        foreach (var track in tracks)
        {
            if (track is RhubarbPlayableTrack rhubarbTrack)
            {
                if (track.name == teacher_say_lipsync_track_name)
                {
                    teacher_lipsync_track = rhubarbTrack;
                }
                if (track.name == student_say_lipsync_track_name)
                {
                    student_lipsync_track = rhubarbTrack;
                }
                continue;
            }

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
                continue;
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
                continue;
            }
        }

        if (teacher_say_track != null && student_say_track != null && teacher_say_signal_track != null &&
            student_say_signal_track != null)
        {
            //生成嘴型图片;
            TryToCreateLipSyncClips(teacher_say_track, teacher_lipsync_track);
            TryToCreateLipSyncClips(student_say_track, student_lipsync_track);
            
            
            ResetSignalTrackByAudioTrack(teacher_say_signal_track, teacher_say_track, teacher_startSignal, teacher_endSignal);
            ResetSignalTrackByAudioTrack(student_say_signal_track, student_say_track, student_startSignal, student_endSignal);
        }
        else
        {
            Debug.LogError("轨道数据不完整");
        }

        forceRefreshTimeline();
    }

    private string ConvertMp3PathToWavPath(string rawMp3DataPath)
    {
        string wavAudioDirPath = Path.Combine(Application.dataPath + "/../", "temp_audio_wavs");
        if (!Directory.Exists(wavAudioDirPath))
        {
            Directory.CreateDirectory(wavAudioDirPath);
        }
        string wavAudioFullPath = Path.Combine(wavAudioDirPath, rawMp3DataPath);
        wavAudioFullPath = Path.ChangeExtension(wavAudioFullPath, ".wav");
        return wavAudioFullPath;
    }

    private void TryToCreateLipSyncClips(AudioTrack audioTrack, RhubarbPlayableTrack rhubarbTrack)
    {
        // 先清除原有嘴型数据;
        var rhubardTrackClips = rhubarbTrack.GetClips();
        foreach (var clip in rhubardTrackClips)
        {
            rhubarbTrack.DeleteClip(clip);
        }
        int index = 0;
        // 获取 Audio 轨道上的所有剪辑
        var clips = audioTrack.GetClips();
        int totalClips = clips.ToArray().Length;
        foreach (var clip in clips)
        {
            index++;
            var startTime = clip.start;
            // var endTime = clip.start + clip.duration;
            AudioClip audioClip = (clip.asset as AudioPlayableAsset).clip;
            if (audioClip != null)
            {
                EditorUtility.DisplayProgressBar("提示", $"正在升成口型数据 {index}/{totalClips}", index / (float)totalClips);
                string audioClipPath = AssetDatabase.GetAssetPath(audioClip);
                string wavPath = ConvertMp3PathToWavPath(audioClipPath);
                string lipsyncPresetPath = wavPath+".lipsync";
                if (!File.Exists(wavPath))
                {
                    RhubarbEditorProcess.ConvertMp3ToWav(audioClipPath, wavPath);
                    if (File.Exists(lipsyncPresetPath))
                    {
                        File.Delete(lipsyncPresetPath);
                    }
                    RhubarbEditorProcess.CreatSimpleRhubarbTrack(wavPath, lipsyncPresetPath);
                }

                SimpleRhubarbTrackData simpleRhubarbTrackData = RhubarbEditorProcess.GetSimpleRhubarbTrack(lipsyncPresetPath);
                //嘴型 clip 创建;
                for (int i = 0; i < simpleRhubarbTrackData.keyframes.Count - 1; i++)
                {
                    SimpleRhubarbKeyframeData keyframe = simpleRhubarbTrackData.keyframes[i];
                    SimpleRhubarbKeyframeData nextKeyframe = simpleRhubarbTrackData.keyframes[i + 1];
                    TimelineClip rhubarbTrackClip = rhubarbTrack.CreateClip<RhubarbPlayableClip>();
                    rhubarbTrackClip.start = startTime + Rhubarb.FrameToTime(keyframe.frame);
                    rhubarbTrackClip.duration = Rhubarb.FrameToTime(nextKeyframe.frame - keyframe.frame);
                    ((RhubarbPlayableClip) rhubarbTrackClip.asset).template.MouthShape = keyframe.phoneme;
                }
            }
        }
        EditorUtility.ClearProgressBar();
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
    
    void AddLipAudioAmplitudeEvent(EventSenderType eventSenderType, SignalTrack signalTrack, double time, float amplitude, SignalAsset signalAsset)
    {
        // 添加到 Signal 轨道
        var courseLipAudioAmplitudeEmitter = signalTrack.CreateMarker<CourseLipAudioAmplitudeEmitter>(time);
        courseLipAudioAmplitudeEmitter.amplitude = amplitude;
        courseLipAudioAmplitudeEmitter.eventSenderType = eventSenderType;
        courseLipAudioAmplitudeEmitter.asset = signalAsset;
    }
#endif
}
