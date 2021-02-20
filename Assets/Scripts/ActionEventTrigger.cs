using System.Collections;
using System.Collections.Generic;
using Scripts.Hacking;
using UnityEngine;

public class ActionEventTrigger : MonoBehaviour
{
    public ActionEventItem[] actionEventItems;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var a in actionEventItems)
        {
            if (a.action.outputGate != null)
            {
                a.action.outputGate.ValueHasChanged += (sender, e) => gate_ValueChanged(sender, e, a);
            }
            else
            {
                a.action.AfterInit += (x, y) => a.action.outputGate.ValueHasChanged += (sender, e) => gate_ValueChanged(sender, e, a);
            }
        }
    }

    private void gate_ValueChanged(object sender, System.EventArgs e, ActionEventItem a)
    {
        if (a.action.outputGate.currentValue)
        {
            a.onOutputTrue.Invoke();
        }
        else
        {
            a.onOutputFalse.Invoke();
        }
    }
}
