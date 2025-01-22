using UnityEngine;
using UnityEngine.Playables;

namespace 测试.测试timeline
{
    public class LightControlBehaviourPzf : PlayableBehaviour
    {
        public Color color = Color.white;
        public float intensity = 1f;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            Light light = playerData as Light;
            if (light != null)
            {
                // Debug.Log("1111111111111111111111  "+color.ToString());
                light.color = color;
                light.intensity = intensity;
            }
        }
        
        public override void PrepareData(Playable playable, FrameData info)
        {
            Debug.Log("PrepareData");
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            Debug.Log("PrepareFrame");
        }

        public override void OnGraphStart(Playable playable)
        {
            Debug.Log("Graph Started");
        }

        public override void OnGraphStop(Playable playable)
        {
            Debug.Log("Graph Stopped");
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            Debug.Log("Clip Played");
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            Debug.Log("Clip Paused");
        }

        public override void OnPlayableCreate(Playable playable)
        {
            Debug.Log("OnPlayableCreate");
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            Debug.Log("OnPlayableDestroy");
        }
    }
}