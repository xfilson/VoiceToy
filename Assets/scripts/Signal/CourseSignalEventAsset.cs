using System;
using UnityEngine;

namespace DefaultNamespace
{
    [Serializable]
    public enum EventSenderType
    {
        None,
        Teacher,
        Student,
    }

    [Serializable]
    [CreateAssetMenu(fileName = "课程信号资源", menuName = "模板资产创建/课程信号资源/事件", order = 1)]
    public class CourseSignalEventAsset : CourseSignalAsset
    {
        public EventSenderType SenderType = EventSenderType.None;
        public bool isGlobalEvent = true;
    }
}