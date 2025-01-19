using System.Collections;
using DefaultNamespace;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SugarFrame.Node
{
    [NodeNote("请求说话", "VoiceToy")]
    public class Say : BaseAction
    {
        [Header("-----------自定义参数-----------")]
        [LabelText("音频")]
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
                OnSayStart(emitTrigger, audioClipPlayer);
            }
            else
            {
                Debug.LogError("找不到AudioSource "+this.gameObject.name);
                RunOver(emitTrigger);
            }
        }

        private void OnSayStart(BaseTrigger emitTrigger, AudioClipPlayer audioClipPlayer)
        {
            CharacterBase character = this.gameObject.GetScriptComponentInHierarchy<CharacterBase>();
            if (character != null)
            {
                character.OnSayStart(this.audioClip, () =>
                {
                    StartCoroutine(OnSayEnd(emitTrigger));
                });
            }
        }

        private IEnumerator OnSayEnd(BaseTrigger emitTrigger)
        {
            CharacterBase character = this.gameObject.GetScriptComponentInHierarchy<CharacterBase>();
            if (character != null)
            {
                character.OnSayEnd();
            }
            //要延迟一帧，不然太快Play下一个动画不生效;
            yield return null;
            RunOver(emitTrigger);        
            EventManager.DispatchEvent(this.dispatchEventWhenPlayComplete, this.dispatchEventParam, isCurGameObjectAsSender?this:null);
        }
    }

}