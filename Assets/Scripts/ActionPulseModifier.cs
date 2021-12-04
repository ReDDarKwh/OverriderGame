using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Hacking;
using Scripts.Actions;
using System;
using Action = Scripts.Actions.Action;

public class ActionPulseModifier : MonoBehaviour
{
    public Action activate;
    public Action desactivate;
    public Action active;

    // Start is called before the first frame update
    void Start()
    {
        EventHandler ah = (object sender, EventArgs args) =>
        {
            if (activate.outputGate.currentValue)
            {
                active.actionGate.SetValue(true);
            }
        };

        EventHandler dh = (object sender, EventArgs args) =>
        {
            if (desactivate.outputGate.currentValue)
            {
                active.actionGate.SetValue(false);
            }
        };

        if (activate.outputGate != null)
        {
            activate.outputGate.ValueHasChanged += ah;
        }
        else
        {
            activate.AfterInit += (x, y) =>
            activate.outputGate.ValueHasChanged += ah;
        }

        if (desactivate.outputGate != null)
        {
            desactivate.outputGate.ValueHasChanged += dh;
        }
        else
        {
            desactivate.AfterInit += (x, y) =>
            desactivate.outputGate.ValueHasChanged += dh;
        }
    }

    public void Desactivate()
    {
        desactivate.actionGate.SetValue(false);
        desactivate.actionGate.SetValue(true);
        desactivate.actionGate.SetValue(false);
    }
}
