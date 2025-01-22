using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEditor;

public static class TimelineAssetCopier
{
    /// <summary>
    /// 复制 TimelineAsset 并更新绑定到新的 GameObject。
    /// </summary>
    /// <param name="originalTimelineAsset">原始 TimelineAsset。</param>
    /// <param name="newGameObject">新的 GameObject，绑定将更新到此 GameObject 或其子对象。</param>
    /// <param name="newPath">新 TimelineAsset 的保存路径。</param>
    public static void CopyTimelineAssetWithBindings(TimelineAsset originalTimelineAsset, GameObject newGameObject, string newPath)
    {
        if (originalTimelineAsset == null)
        {
            Debug.LogError("Original TimelineAsset is null.");
            return;
        }

        if (newGameObject == null)
        {
            Debug.LogError("New GameObject is null.");
            return;
        }

        if (string.IsNullOrEmpty(newPath))
        {
            Debug.LogError("Invalid new path for TimelineAsset.");
            return;
        }

        // 获取原始 TimelineAsset 的路径
        string originalPath = AssetDatabase.GetAssetPath(originalTimelineAsset);

        // 复制 TimelineAsset 到新路径
        AssetDatabase.CopyAsset(originalPath, newPath);
        AssetDatabase.Refresh();

        // 加载新复制的 TimelineAsset
        TimelineAsset copiedTimelineAsset = AssetDatabase.LoadAssetAtPath<TimelineAsset>(newPath);
        if (copiedTimelineAsset == null)
        {
            Debug.LogError("Failed to load the copied TimelineAsset.");
            return;
        }

        // 更新绑定到新的 GameObject 或其子对象
        UpdateBindings(copiedTimelineAsset, newGameObject);

        // 保存新的 TimelineAsset
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"TimelineAsset copied and bindings updated successfully to: {newPath}");
    }

    private static void UpdateBindings(TimelineAsset timelineAsset, GameObject newGameObject)
    {
        // 创建一个临时的 PlayableDirector 来管理绑定
        GameObject tempDirectorObject = new GameObject("TempPlayableDirector");
        PlayableDirector playableDirector = tempDirectorObject.AddComponent<PlayableDirector>();
        playableDirector.playableAsset = timelineAsset;

        // 获取当前绑定信息
        var originalBindings = timelineAsset.outputs;

        // 清空旧的绑定
        var serializedObject = new SerializedObject(playableDirector);
        var bindingsProperty = serializedObject.FindProperty("m_SceneBindings");
        bindingsProperty.ClearArray();

        // 更新绑定到新的 GameObject 或其子对象
        foreach (var binding in originalBindings)
        {
            UnityEngine.Object newBindingObject = FindObjectInNewHierarchy(newGameObject, binding.sourceObject.name);
            if (newBindingObject != null)
            {
                // 设置新的绑定
                int index = bindingsProperty.arraySize;
                bindingsProperty.InsertArrayElementAtIndex(index);
                var element = bindingsProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("key").objectReferenceValue = binding.sourceObject;
                element.FindPropertyRelative("value").objectReferenceValue = newBindingObject;
            }
        }

        // 应用修改
        serializedObject.ApplyModifiedProperties();
        Object.DestroyImmediate(tempDirectorObject);
    }

    private static UnityEngine.Object FindObjectInNewHierarchy(GameObject newGameObject, string path)
    {
        Transform foundTransform = newGameObject.transform.Find(path);
        return foundTransform != null ? foundTransform.gameObject : null;
    }
}