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
        Timer timer = new(1, true);
        Debug.Log(timer);
        timer.Initialize();
        timer.TimerEnd += () => Debug.Log("Timer ended");
    }

    private void OnDestroy()
    {
        
    }
}
