using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace 测试.测试timeline
{
    public class LightCOntrollerBehaviourMixerPzf : PlayableBehaviour
    {
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            int inputCount = playable.GetInputCount();
            Color resultColor = Color.black;
            for (int i = 0; i < inputCount; i++)
            {
                var tmpPlayable = playable.GetInput(i);
                var purePlayable = ((ScriptPlayable<LightControlBehaviourPzf>)tmpPlayable).GetBehaviour();
                float weight = playable.GetInputWeight(i);
                resultColor += purePlayable.color * weight;
            }

            (playerData as Light).color = resultColor;
        }
    }
}