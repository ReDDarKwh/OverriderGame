using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Hacking;
using UnityEngine;
using System.Linq;
using Bolt;

public class AlarmAction : Action
{
    public ExternalLogicAction activate;
    public ExternalLogicAction desactivate;
    public Animator animator;
    public GameObject alternativeTarget;
    public Enemy[] assignedGuards;
    private DataGate targetsDataInput;


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

        targetsDataInput = new DataGate { name = "targets" };
        dataGates.Add(targetsDataInput);

        activate.outputGate.ValueHasChanged += (object sender, EventArgs args) =>
        {
            AttractClosestGuard();
        };
    }

    private void AttractClosestGuard()
    {
        var target = targetsDataInput.GetData<GameObject>().FirstOrDefault() ?? alternativeTarget;

        var minDis = float.MaxValue;
        Enemy minEnemy = null;
        foreach (var guard in assignedGuards.Where(x => x != null))
        {
            float magnitude = (transform.position - guard.transform.position).magnitude;
            if (magnitude < minDis && guard.gameObject != target)
            {
                minEnemy = guard;
                minDis = magnitude;
            }
        }

        if (minEnemy != null)
        {
            CustomEvent.Trigger(minEnemy.gameObject, "NoiseHeard", target.transform.position);
        }

    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Active", outputGate.currentValue);
    }
}
