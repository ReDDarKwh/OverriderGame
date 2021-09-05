using System;
using System.Collections.Generic;
using Hsm;
using Scripts.States;
using UnityEngine;

class TargetAlertSuperState : SuperState
{
    public TargetAlertSuperState(StateMachine sm, HSM root, string name) : base(sm, root, name)
    {
    }

    public override void Init(StateMachine sm, HSM root)
    {
        var start = AddState(root.GetComponent<EmptyState>(), "start");
        var interacting = new InteractSuperState(sm, root, "interacting").sub;
        var fleeing = new FleeingSuperState(sm, root, "fleeing").sub;

        start.AddEnterHandler(interacting, null, (EventData data) => {
            root.memory.Set("interactionObject", ((TargetHSM)root).alertSwitch, MemoryType.GameObject);
        });

        interacting.AddHandler("interactionDone", fleeing);
    }
    
}