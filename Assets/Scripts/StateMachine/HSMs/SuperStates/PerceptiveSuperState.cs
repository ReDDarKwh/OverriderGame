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
        var interact = new InteractSuperState(sm, root, "interact");

        idle.AddUpdateHandler(unsure, EventRepo.TargetInTargetList);
        unsure.AddUpdateHandler(curious, EventRepo.TargetOutOfTargetList);
        curious.AddUpdateHandler(searching.sub, EventRepo.Timeout(5));

        idle.AddEnterHandler(searching.sub, EventRepo.HasTarget, (Dictionary<string, object> data) =>
        {
            var memory = HSM.GetRoot(data).memory;
            var target = memory.Get<GameObject>("target");
            memory.Set("targetPos", target.transform.position);
        });

        idle.AddHandler("objectNoiseHeard", interact.sub, TransitionKind.External, (Dictionary<string, object> data) =>
        {
            var memory = HSM.GetRoot(data).memory;
            var interactable = HSM.GetVar<GameObject>("subject", data);
            memory.Set("interactionObject", interactable);
        },
        (Dictionary<string, object> data) =>
        {
            var investigator = HSM.GetRoot(data).GetComponent<Investigator>();
            var interactable = HSM.GetVar<GameObject>("subject", data);
            return investigator.CanBeInvestigated(interactable);
        });

        interact.sub.AddHandler("interactionDone", idle);
        interact.sub.AddUpdateHandler(unsure, EventRepo.TargetInTargetList);

        searching.sub.AddHandler("searchDone", idle);
        searching.sub.AddUpdateHandler(unsure, EventRepo.TargetInTargetList);

    }
}