using UnityEngine;

public static class GameObjectExtensions
{
    /// <summary>
    /// 尝试获取组件，如果不存在则添加并返回
    /// </summary>
    /// <typeparam name="T">要获取或添加的组件类型</typeparam>
    /// <param name="gameObject">当前GameObject</param>
    /// <returns>获取或添加的组件</returns>
    public static T TryGetComponent<T>(this GameObject gameObject) where T : Component
    {
        // 尝试获取组件
        T component = gameObject.GetComponent<T>();

        // 如果组件不存在，则添加并返回
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }

        return component;
    }
    
    /// <summary>
    /// 获取当前GameObject或其父对象上的指定脚本组件
    /// </summary>
    /// <typeparam name="T">要获取的脚本组件类型</typeparam>
    /// <param name="gameObject">当前GameObject</param>
    /// <returns>找到的脚本组件，如果未找到则返回null</returns>
    public static T GetScriptComponentInHierarchy<T>(this GameObject gameObject) where T : Component
    {
        // 首先尝试在当前对象上获取脚本组件
        T component = gameObject.GetComponent<T>();
        if (component != null)
        {
            return component;
        }

        // 如果当前对象上没有该脚本组件，则递归地在父对象上查找
        Transform parent = gameObject.transform.parent;
        while (parent != null)
        {
            component = parent.GetComponent<T>();
            if (component != null)
            {
                return component;
            }
            parent = parent.parent;
        }

        // 如果在当前对象及其所有父对象上都没有找到该脚本组件，则返回null
        return null;
    }
    
    
}