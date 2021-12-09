using System.Collections.Generic;
using Hsm;
using Scripts.States;
using UnityEngine;

class InteractSuperState : SuperState
{
    public InteractSuperState(StateMachine sm, HSM root, string name) : base(sm, root, name)
    {
    }

    public override void Init(StateMachine sm, HSM root)
    {
        var start = AddState(root.GetComponent<EmptyState>(), "start");
        var go = new GotoSuperState(sm, root, "goto");
        var interacting = AddState(root.GetComponent<InteractingState>(), "interacting");
        var stuck = AddState(root.GetComponent<EmptyState>(), "stuck");

        start.AddEnterHandler(go.sub, null, (data) =>
        {
            var memory = HSM.GetRoot(data).memory;
            var interactionObject = memory.Get<GameObject>("interactionObject", false).GetComponent<Interactable>();
            HSM.SetUpGoto(
                memory,
                null,
                interactionObject.interactionPos == null ? interactionObject.transform : interactionObject.interactionPos,
                "investigating",
                false
            );
        });

        go.sub.AddHandler("isAtPosition", interacting);

        go.sub.AddHandler("isStuck", stuck, (EventData data) => {
            root.TriggerEvent("interactionDone");
        });

    }
}