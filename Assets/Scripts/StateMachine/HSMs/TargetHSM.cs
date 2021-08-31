using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hsm;
using Scripts.States;

public class TargetHSM : HSM
{
    public GameObject alertSwitch;
    
    public override void Init(StateMachine sm, HSM root)
    {
        var alive = new TargetAliveSuperState(sm, root, "alive");
        var dead = AddState(root.GetComponent<DeadState>(), "dead");

        alive.sub.AddHandler("died", dead);
    }
}
