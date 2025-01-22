using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEditor;

public static class TimelineAssetCopier
{
    public static void CopyPlayableDirectorTimeline(PlayableDirector rawPlayableDirector, PlayableDirector targetPlayableDirector)
    {
        var targetSO = new SerializedObject(targetPlayableDirector);
        var targetSceneBindings = targetSO.FindProperty("m_SceneBindings");
        var bindingsKv = new Dictionary<Object, Object>();
        for (var i = 0; i < targetSceneBindings.arraySize; i++)
        {
            var bindingElement = targetSceneBindings.GetArrayElementAtIndex(i);
            // key是源文件的轨道对象，value是当前文件的绑定value
            var key = bindingElement.FindPropertyRelative("key").objectReferenceValue;
            var val = bindingElement.FindPropertyRelative("value").objectReferenceValue;

            if (key == null)
            {
                continue;
            }
            bindingsKv.Add(key, val);
        }
        // 移除无用的绑定
        while (targetSceneBindings.arraySize > 0)
        {
            targetSceneBindings.DeleteArrayElementAtIndex(0);
        }
        targetSO.ApplyModifiedProperties();

        // // 设置有效绑定
        // foreach (var kv in bindingsKv)
        // {
        //     targetPlayableDirector.SetGenericBinding(kv.Key, kv.Value);
        // }
        
        // 设置有效绑定 (新老timeline资源outputs顺序完全一致)
        var lstout = rawPlayableDirector.playableAsset.outputs.ToList();
        var newout = targetPlayableDirector.playableAsset.outputs.ToList();
        for (int i = 0; i < lstout.Count; i++)
        {
            var lstTrack = lstout[i].sourceObject as TrackAsset;
            var newTrack = newout[i].sourceObject as TrackAsset;

            if (bindingsKv.ContainsKey(lstTrack))
            {
                targetPlayableDirector.SetGenericBinding(newTrack, bindingsKv[lstTrack]);
            }
        }
    }

    
}