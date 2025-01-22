using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace 测试.测试timeline
{
    [TrackColor(0, 0.5f, 0)]
    [TrackClipType(typeof(LightControlAssetPzf))]
    [TrackBindingType(typeof(Light))]
    public class LightControlTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<LightCOntrollerBehaviourMixerPzf>.Create(graph, inputCount);
        }
    }
}