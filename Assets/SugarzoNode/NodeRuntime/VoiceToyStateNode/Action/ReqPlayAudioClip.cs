using DefaultNamespace;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SugarFrame.Node
{
    [NodeNote("播放音频", "VoiceToy")]
    public class ReqPlayAudioClip : BaseAction
    {
        [Header("-----------自定义参数-----------")]
        [LabelText("PlayAudioClip")]
        public AudioClip audioClip;
        [LabelText("当前gameobject作为sender")]
        public bool isCurGameObjectAsSender = true;
        [LabelText("播放完毕后发送的事件")]
        public string dispatchEventWhenPlayComplete;
        [LabelText("派发事件参数")]
        public string dispatchEventParam;

        public override void RunningLogic(BaseTrigger emitTrigger)
        {
            //Write Logic
            AudioClipPlayer audioClipPlayer = this.gameObject.GetScriptComponentInHierarchy<AudioClipPlayer>();
            if (audioClipPlayer != null)
            {
                audioClipPlayer.PlayAudioClip(this.audioClip, () =>
                {
                    RunOver(emitTrigger);
                    EventManager.DispatchEvent(this.dispatchEventWhenPlayComplete, this.dispatchEventParam, isCurGameObjectAsSender?this:null);
                });
            }
            else
            {
                Debug.LogError("找不到AudioSource "+this.gameObject.name);
                RunOver(emitTrigger);
            }
        }
    }
}