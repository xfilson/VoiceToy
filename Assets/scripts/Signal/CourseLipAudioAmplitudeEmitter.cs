using System;
using UnityEngine.Timeline;

namespace DefaultNamespace
{
    [Serializable]
    public class CourseLipAudioAmplitudeEmitter : SignalEmitter
    {
        public EventSenderType eventSenderType = EventSenderType.None;
        public float amplitude;
    }
}