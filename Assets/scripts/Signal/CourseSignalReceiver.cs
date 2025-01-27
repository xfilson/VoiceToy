using UnityEngine;
using UnityEngine.Playables;

namespace DefaultNamespace
{
    public class CourseSignalReceiver : MonoBehaviour, INotificationReceiver
    {
        public void OnNotify(Playable origin, INotification notification, object context)
        {
            CourseSignalEmitter signalEmitter = notification as CourseSignalEmitter;

            #region 事件类型的信号;

            if (signalEmitter && signalEmitter.asset is CourseSignalEventAsset)
            {
                CourseSignalEventAsset signalEventAsset = signalEmitter.asset as CourseSignalEventAsset;
                string eventName = signalEventAsset.param;
                if (string.IsNullOrEmpty(eventName))
                {
                    eventName = signalEventAsset.name;
                }

                object senderObj = null;
                if (!signalEventAsset.isGlobalEvent)
                {
                    switch (signalEventAsset.SenderType)
                    {
                        case EventSenderType.Teacher:
                            senderObj = CourceManager.Instance.character_teacher;
                            break;
                        case EventSenderType.Student:
                            senderObj = CourceManager.Instance.character_student;
                            break;
                    }   
                }
                Debug.Log("课程信号发射: "+eventName);
                EventManager.DispatchEvent(eventName, null, senderObj);
            }
            
            #endregion

            #region 嘴型振幅的信号
            CourseLipAudioAmplitudeEmitter lipAudioAmplitudeEmitter = notification as CourseLipAudioAmplitudeEmitter;
            if (lipAudioAmplitudeEmitter && lipAudioAmplitudeEmitter.asset is CourseLipAudioAmplitudeAsset)
            {
                float amplitude = lipAudioAmplitudeEmitter.amplitude;
                switch (lipAudioAmplitudeEmitter.eventSenderType)
                {
                    case EventSenderType.Student:
                        CourceManager.Instance.character_student.RandomZuixingConfigItem();
                        break;
                    case EventSenderType.Teacher:
                        CourceManager.Instance.character_teacher.RandomZuixingConfigItem();
                        break;
                }
            }

            #endregion
        }
    }
}