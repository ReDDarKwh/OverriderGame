using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Hacking;
using UnityEngine;

public class TargetingAction : Action
{
    internal IEnumerable<Transform> targets;
    private Quaternion initialRotation;
    private DataGate targetsDataInput;

    internal override void OnStart()
    {
        initialRotation = transform.rotation;

        targetsDataInput = new DataGate { name = "targets" };
        dataGates.Add(targetsDataInput);

        targetsDataInput.ValueHasChanged += targetsDataInput_ValueChanged;
    }

    private void targetsDataInput_ValueChanged(object sender, System.EventArgs e)
    {
        this.targets = targetsDataInput.GetData<GameObject>().Select(x => x.transform);
    }

    // Update is called once per frame
    void Update()
    {
        var target = targets?.FirstOrDefault();
        if (target != null)
        {
            var turretToTarget = target.position - transform.position;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, turretToTarget) * Quaternion.Euler(0, 0, 90);
            actionGate.SetValue(true);
        }
        else
        {
            actionGate.SetValue(false);
            transform.rotation = initialRotation;
        }
    }
}
