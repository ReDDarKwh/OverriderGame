using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchAction : Action
{
    public float onTime;
    public AudioSource audioSource;
    private float lastOnTime;

    public void Toggle()
    {
        actionGate.SetValue(!actionGate.currentValue);

        audioSource.Play();

        if (actionGate.currentValue)
        {
            lastOnTime = Time.time;
        }
    }

    internal override void OnStart()
    {

    }


    void Update()
    {
        if (Time.time - lastOnTime > onTime && actionGate.currentValue)
        {
            actionGate.SetValue(false);
        }
    }
}
