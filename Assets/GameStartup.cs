using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartup : MonoBehaviour
{
    private void Awake()
    {
        Application.runInBackground = true;
    }
}
