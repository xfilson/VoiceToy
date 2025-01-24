using System;
using UnityEngine;
using UnityEngine.Timeline;

namespace DefaultNamespace
{
    [Serializable]
    [CreateAssetMenu(fileName = "课程信号资源", menuName = "模板资产创建/课程信号资源/字符串参数", order = 1)]
    public class CourseSignalAsset : SignalAsset
    {
        public string param;
    }
}