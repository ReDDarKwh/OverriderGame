using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmAction : Action
{
    public ExternalLogicAction activate;
    public ExternalLogicAction desactivate;
    public Animator animator;

    internal override void OnStart()
    {
        var alarmManager = GameObject.FindGameObjectWithTag("SceneManager")
             .GetComponent<AlarmManager>();

        alarmManager.gate.Connect(this.actionGate);

        activate.outputGate.ValueHasChanged += (object sender, EventArgs args) =>
        {
            if (activate.outputGate.currentValue)
            {
                alarmManager.gate.SetValue(true);
            }
        };

        desactivate.outputGate.ValueHasChanged += (object sender, EventArgs args) =>
        {
            if (desactivate.outputGate.currentValue)
            {
                alarmManager.gate.SetValue(false);
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Active", outputGate.currentValue);
    }
}
