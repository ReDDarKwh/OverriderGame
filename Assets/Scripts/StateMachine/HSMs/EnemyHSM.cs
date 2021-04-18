using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hsm;
using Scripts.States;

public class EnemyHSM : HSM
{
    public override void Init(StateMachine sm, HSM root)
    {
        var alive = new EnemyAliveSuperState(sm, root, "alive");
        var dead = AddState(root.GetComponent<DeadState>(), "dead");

        alive.sub.AddHandler("died", dead);
    }
}
