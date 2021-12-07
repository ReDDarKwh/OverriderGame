using System.Collections.Generic;
using Hsm;
using Scripts.States;
using UnityEngine;

class StationnarySuperState : SuperState
{
    public StationnarySuperState(StateMachine sm, HSM root, string name) : base(sm, root, name)
    {
    }

    public override void Init(StateMachine sm, HSM root)
    {

        var start = AddState(root.GetComponent<PatrollingState>(), "start");
        var go = new GotoSuperState(sm, root, "goto");
        var stuck = AddState(root.GetComponent<EmptyState>(), "stuck");
        var atPos = AddState(root.GetComponent<EmptyState>(), "atPos");

        start.AddHandler("hasPatrolPoint", go.sub);

        go.sub.AddHandler("isAtPosition", atPos, TransitionKind.External, (EventData data) => {
            root.GetComponent<PatrollingState>().AtStationnaryOutpost();
        });

        atPos.AddUpdateHandler(start, EventRepo.Timeout(5));

        go.sub.AddHandler("isStuck", stuck);
        stuck.AddUpdateHandler(start, EventRepo.Timeout(5));
    }
}