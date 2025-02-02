using System;
using UnityEngine;
using UnityEngine.Timeline;

namespace DefaultNamespace
{
    [Serializable]
    public enum EffectLayerType
    {
        Back,
        Front,
    }
    
    [Serializable]
    [CreateAssetMenu(fileName = "课程信号资源", menuName = "模板资产创建/课程信号资源/特效预制体", order = 1)]
    public class CourseSignalEffectAsset : SignalAsset
    {
        public EffectLayerType layerType = EffectLayerType.Front;
        public GameObject effectPrefab;
    }
}