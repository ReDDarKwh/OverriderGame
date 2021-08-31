using System.Collections.Generic;
using Hsm;
using Scripts.States;
using UnityEngine;

class IdleSuperState : SuperState
{
    public IdleSuperState(StateMachine sm, HSM root, string name) : base(sm, root, name)
    {
    }

    public override void Init(StateMachine sm, HSM root)
    {
        var wait = AddState(root.GetComponent<EmptyState>(), "wait");
        var start = AddState(root.GetComponent<EmptyState>(), "start");
        var patrolling = new PatrollingSuperState(sm, root, "patrolling");
        var stationnary = new StationnarySuperState(sm, root, "stationnary");

        wait.AddUpdateHandler(start, EventRepo.Timeout(0.1f));

        start.AddEnterHandler(patrolling.sub, (EventData data) => {
            return root.GetComponent<PatrollingState>().stationnaryTransform == null;
        });

        start.AddEnterHandler(stationnary.sub, (EventData data) => {
            return root.GetComponent<PatrollingState>().stationnaryTransform != null;
        });
    }
}