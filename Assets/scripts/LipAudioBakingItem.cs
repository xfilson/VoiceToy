using UnityEngine;

/// <summary>
/// 嘴型 Audio 烘焙数据;
/// </summary>
public class LipAudioBakingItem
{
    public float[] amplitudeData;

    public void StartBake(AudioClip clip)
    {
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        amplitudeData = new float[samples.Length];
        for (int i = 0; i < samples.Length; i++)
        {
            amplitudeData[i] = Mathf.Abs(samples[i]);
        }

        Debug.Log($"Amplitude data baked successfully.   {clip.name}");
    }
}