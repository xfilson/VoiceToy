using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
// var settingsPanel = UIManager.Instance.GetPanel<SettingsPanel>("Settings");
// UIManager.Instance.ClosePanel(settingsPanel);
/// </summary>
public class UIManager : SingletonMono<UIManager>
{
    // UI根节点配置
    [SerializeField] private Transform _canvasRoot; 
    [SerializeField] private GameObject _loadingPanelPrefab;
    
    private Dictionary<string, BasePanel> _panelCache = new Dictionary<string, BasePanel>();
    private Stack<BasePanel> _panelStack = new Stack<BasePanel>();
    
    // 初始化UI画布
    void Awake()
    {
        if (_canvasRoot == null)
            _canvasRoot = GameObject.Find("Canvas")?.transform ?? CreateCanvas();
    }

    // 异步打开面板（支持远程/本地）
    public async UniTask<T> OpenPanelAsync<T>(string panelKey, string pathOrUrl) where T : BasePanel
    {
        // 显示加载界面
        var loadingPanel = ShowLoadingPanel();

        try
        {
            // 通过下载管理器获取资源
            T prefab = await DownloadManager.Instance.AddDownload<T>(pathOrUrl,
                progress =>
                {
                    if (loadingPanel != null)
                    {
                        loadingPanel.UpdateProgress(progress);
                    }
                });

            // 实例化并初始化面板
            var panel = InstantiatePanel(prefab, panelKey);
            _panelStack.Push(panel);
            return panel;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        finally
        {
            // 隐藏加载界面
            HideLoadingPanel(loadingPanel);
        }
        return null;
    }

    // 关闭当前面板
    public void CloseCurrentPanel()
    {
        if (_panelStack.Count > 0)
        {
            var panel = _panelStack.Pop();
            _panelCache.Remove(panel.PanelKey);
            DestroyPanel(panel);
        }
    }
    
    public void ClosePanel(BasePanel targetPanel)
    {
        if (targetPanel == null) return;

        // 从缓存中移除
        if (_panelCache.ContainsKey(targetPanel.PanelKey))
        {
            _panelCache.Remove(targetPanel.PanelKey);
        }

        // 从栈中移除（需要遍历查找）
        var tempStack = new Stack<BasePanel>();
        bool foundInStack = false;
    
        while (_panelStack.Count > 0)
        {
            var panel = _panelStack.Pop();
            if (panel == targetPanel)
            {
                foundInStack = true;
                break;
            }
            tempStack.Push(panel);
        }
    
        // 还原非目标面板
        while (tempStack.Count > 0)
        {
            _panelStack.Push(tempStack.Pop());
        }

        // 如果找到则销毁
        if (foundInStack)
        {
            DestroyPanel(targetPanel);
        }
    }
    
    // 扩展获取面板方法
    public T GetPanel<T>(string panelKey) where T : BasePanel
    {
        return _panelCache.TryGetValue(panelKey, out var panel) ? panel as T : null;
    }

    private void DestroyPanel(BasePanel panel)
    {
        panel.OnClose(); // 触发关闭回调
        Destroy(panel.gameObject);
    
        // 如果有子面板需要联动关闭可以在这里扩展
        // foreach(var child in panel.ChildPanels){
        //     ClosePanel(child);
        // }
    }

    private Transform CreateCanvas()
    {
        var go = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler));
        var canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        return go.transform;
    }

    private T InstantiatePanel<T>(T prefab, string panelKey) where T : BasePanel
    {
        if (_panelCache.TryGetValue(panelKey, out var cachedPanel))
        {
            cachedPanel.gameObject.SetActive(true);
            return (T)cachedPanel;
        }

        var panel = Instantiate(prefab, _canvasRoot);
        panel.transform.localPosition = Vector3.zero;
        panel.transform.localRotation = Quaternion.identity;
        panel.transform.localScale = Vector3.one;
        panel.Initialize(panelKey);
        _panelCache.Add(panelKey, panel);
        return panel;
    }

    private LoadingPanel ShowLoadingPanel()
    {
        if (_loadingPanelPrefab != null)
        {
            var panel = Instantiate(_loadingPanelPrefab, _canvasRoot).GetComponent<LoadingPanel>();
            panel.transform.SetAsLastSibling();
            return panel;   
        }
        return null;
    }

    private void HideLoadingPanel(LoadingPanel panel)
    {
        if (panel != null)
        {
            Destroy(panel.gameObject);   
        }
    }
}