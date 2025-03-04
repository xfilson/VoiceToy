using UnityEngine;

public class LoadingPanel : BasePanel 
{
    public void UpdateProgress(float progress)
    {
        if (progress >= 1) OnClose();
    }
}