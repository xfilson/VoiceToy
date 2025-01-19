using Sirenix.OdinInspector;
using UnityEngine;

namespace SugarFrame.Node
{
    [NodeNote("游戏事件监听器", "VoiceToy")]
    public class EventTypeTrigger : BaseTrigger
    {
        [Header("-----------自定义参数-----------")]
        [LabelText("当前gameobject作为sender")]
        public bool isCurGameObjectAsSender = true;
        public string ListenEventType;
        //Called on Enable
        public override void RegisterSaveTypeEvent()
        {
            //EventManager.StartListening("",Execute);
            EventManager.AddListener(this.ListenEventType, this.isCurGameObjectAsSender ? this.gameObject : null, onExecuteCallback);
        }

        //Called on DisEnable
        public override void DeleteSaveTypeEvent()
        {
            //EventManager.StopListening("",Execute);
            EventManager.RemoveListener(this.ListenEventType, this.isCurGameObjectAsSender?this.gameObject:null, onExecuteCallback);
        }

        private void onExecuteCallback(string eventType, object sender, object param)
        {
            Execute();
        }
    }

}