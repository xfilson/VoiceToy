using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace 测试.测试timeline
{
    // public class LightControlAssetPzf : PlayableAsset
    // {
    //     public ExposedReference<Light> light;
    //     public Color color = Color.white;
    //     public float intensity = 1.0f;
    //
    //     public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    //     {
    //         var playable = ScriptPlayable<LightControlBehaviourPzf>.Create(graph);
    //
    //         var lightControlBehaviour = playable.GetBehaviour();
    //         lightControlBehaviour.light = light.Resolve(graph.GetResolver());
    //         lightControlBehaviour.color = color;
    //         lightControlBehaviour.intensity = intensity;
    //
    //         return playable;
    //     }
    // }
    
    public class LightControlAssetPzf : PlayableAsset
    {
        //public ExposedReference<Light> light;
        public Color color = Color.white;
        public float intensity = 1f;

        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<LightControlBehaviourPzf>.Create(graph);

            var lightControlBehaviour = playable.GetBehaviour();
            //lightControlBehaviour.light = light.Resolve(graph.GetResolver());
            lightControlBehaviour.color = color;
            lightControlBehaviour.intensity = intensity;

            return playable;
        }
    }
}