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
            a.action.outputGate.ValueHasChanged += (sender, e) => gate_ValueChanged(sender, e, a);
        }
    }

    private void gate_ValueChanged(object sender, System.EventArgs e, ActionEventItem a)
    {
        var gate = (AbstractGate)sender;
        if (gate.currentValue)
        {
            a.onOutputTrue.Invoke();
        }
        else
        {
            a.onOutputFalse.Invoke();
        }
    }
}
