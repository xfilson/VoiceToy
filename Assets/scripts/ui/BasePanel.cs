// 基础面板类（扩展版）

using UnityEngine;

public class BasePanel : MonoBehaviour
{
    public string PanelKey { get; private set; }
    
    public virtual void Initialize(string key)
    {
        PanelKey = key;
        OnInitialize();
    }

    public virtual void OnInitialize() {}    // 初始化逻辑
    public virtual void OnOpen() {}           // 打开时调用
    public virtual void OnClose() {}          // 关闭时调用
}