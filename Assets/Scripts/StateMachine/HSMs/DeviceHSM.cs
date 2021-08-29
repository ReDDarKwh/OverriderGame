using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hsm;
using Scripts.States;

public class DeviceHSM : HSM
{
    public override void Init(StateMachine sm, HSM root)
    {
        var alive = AddState(root.GetComponent<EmptyState>(), "alive");
        var dead = AddState(root.GetComponent<DeadState>(), "dead");

        alive.AddHandler("died", dead);
    }
}
