using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestClick : MonoBehaviour
{
    public string uipath;

    public void OpenUI()
    {
        UIManager.Instance.OpenPanelAsync<BasePanel>("pzf", uipath);
    }
}
