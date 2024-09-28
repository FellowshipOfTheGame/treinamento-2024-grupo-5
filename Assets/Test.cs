using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mantega;

public class Test : MonoBehaviour
{
    private void Start()
    {
        f();
    }

    private void f()
    {
        Inter timer = new();
        timer.timer.Initialize();
        timer.timer.TimerEnd += () => Debug.Log("Timer ended");
    }

    private void OnDestroy()
    {
        
    }
}
