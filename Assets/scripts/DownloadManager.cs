using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

public class DownloadManager : SingletonMono<DownloadManager>
{
    private readonly List<UniTask> _activeTasks = new();
    private readonly SemaphoreSlim _concurrencySemaphore = new(3); // 最大并发数3
    
    public async UniTask<T> AddDownload<T>(string pathOrUrl, Action<float> onProgress = null) where T : Object
    {
        await _concurrencySemaphore.WaitAsync();
        try
        {
            // 直接返回加载结果，而不是存储 UniTask
            return await LoadResourceAsync<T>(pathOrUrl, onProgress);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        finally
        {
            _concurrencySemaphore.Release();
        }
        return null;
    }

    // 新增：异步加载Resources资源
    private async UniTask<T> LoadResourceAsync<T>(string resourcePath, Action<float> onProgress) where T : Object
    {
        ResourceRequest request = Resources.LoadAsync<T>(resourcePath);
        while (!request.isDone)
        {
            onProgress?.Invoke(request.progress);
            await UniTask.Yield();
        }
        onProgress?.Invoke(1f);
        return (T)request.asset;
    }
}