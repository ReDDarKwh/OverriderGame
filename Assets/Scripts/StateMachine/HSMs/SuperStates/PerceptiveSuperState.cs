using System.Collections.Generic;
using Hsm;
using Scripts.States;
using UnityEngine;

class PerceptiveSuperState : SuperState
{

    public PerceptiveSuperState(StateMachine sm, HSM root, string name) : base(sm, root, name)
    {
    }

    public override void Init(StateMachine sm, HSM root)
    {
        var idle = AddState(root.GetComponent<EmptyState>(), "idle");
        var unsure = AddState(root.GetComponent<UnsureState>(), "unsure");
        var curious = AddState(root.GetComponent<CuriousState>(), "curious");
        var searching = new SearchingSuperState(sm, root, "searching");

        idle.AddUpdateHandler(unsure, EventRepo.TargetInTargetList);
        unsure.AddUpdateHandler(curious, EventRepo.TargetOutOfTargetList);
        curious.AddUpdateHandler(searching.sub, EventRepo.Timeout(5));

        idle.AddEnterHandler(searching.sub, EventRepo.HasTarget, (Dictionary<string, object> data) => {
            var memory = HSM.GetRoot(data).memory;
            var target = memory.Get<GameObject>("target");
            memory.Set("targetPos", target.transform.position);
        });

        searching.sub.AddHandler("searchDone", idle);
        searching.sub.AddUpdateHandler(unsure, EventRepo.TargetInTargetList);

    }
}