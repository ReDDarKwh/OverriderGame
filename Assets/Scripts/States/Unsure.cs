using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;
using System.Linq;

public class Unsure : MonoBehaviour
{
    public ExternalLogicAction chasingAction;
    private GameObject target;

    public void StateEnter()
    {
        var lastTarget = Variables.Object(gameObject).Get<GameObject>("target");
        var i = 0;
        do
        {
            target = chasingAction.dataInputs["Targets"].ElementAt(Random.Range(0, chasingAction.dataInputs["Targets"].Count));
            i++;
        }
        while (lastTarget == target && i < 10);

        if (lastTarget == target)
        {
            CustomEvent.Trigger(this.gameObject, "NothingToClean");
        }

        Variables.Object(gameObject).Set("target", target);
    }
}
