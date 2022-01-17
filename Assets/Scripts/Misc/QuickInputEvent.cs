using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuickInputEvent : MonoBehaviour
{
    public string inputName;
    public UnityEvent onButtonDown;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown(inputName)){
            onButtonDown.Invoke();
        }
    }
}
