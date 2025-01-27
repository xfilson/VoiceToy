using System;
using UnityEngine;
using UnityEngine.Timeline;

namespace DefaultNamespace
{
    [Serializable]
    [CreateAssetMenu(fileName = "课程信号资源", menuName = "模板资产创建/课程信号资源/嘴型音频振幅", order = 1)]
    public class CourseLipAudioAmplitudeAsset : SignalAsset
    {
        public string param;
    }
}