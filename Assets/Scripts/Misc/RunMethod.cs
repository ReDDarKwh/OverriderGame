using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RunMethod : MonoBehaviour
{
    public UnityEvent action;
    // Start is called before the first frame update
    void Run()
    {
        action.Invoke();
    }
}
