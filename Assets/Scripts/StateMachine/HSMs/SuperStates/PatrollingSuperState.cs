using System.Collections.Generic;
using Hsm;
using Scripts.States;
using UnityEngine;

class PatrollingSuperState : SuperState
{
    public PatrollingSuperState(StateMachine sm, HSM root, string name) : base(sm, root, name)
    {
    }

    public override void Init(StateMachine sm, HSM root)
    {

        var start = AddState(root.GetComponent<PatrollingState>(), "start");
        var go = new GotoSuperState(sm, root, "goto");
        var stuck = AddState(root.GetComponent<EmptyState>(), "stuck");

        start.AddHandler("hasPatrolPoint", go.sub);
        go.sub.AddHandler("isAtPosition", start);

        go.sub.AddHandler("isStuck", stuck);
        stuck.AddUpdateHandler(start, EventRepo.Timeout(0.5f));
    }
}