using System.Collections.Generic;
using Hsm;
using Scripts.States;
using UnityEngine;

class FleeingSuperState : SuperState
{
    public FleeingSuperState(StateMachine sm, HSM root, string name) : base(sm, root, name)
    {
    }

    public override void Init(StateMachine sm, HSM root)
    {
        var fleeing = AddState(root.GetComponent<FleeState>(), "fleeing");
        var hidding = new HiddingSuperState(sm, root, "hidding").sub;

        hidding.AddHandler("noiseHeard", fleeing, TransitionKind.External, null, EventRepo.Timeout(3));
        hidding.AddUpdateHandler(fleeing, EventRepo.Concat(EventRepo.Timeout(3), EventRepo.TargetInTargetList));
        hidding.AddHandler("isStuck", fleeing);
        fleeing.AddHandler("flee", hidding, TransitionKind.External, (EventData data) => {
            var pos = data.GetVar<Vector3>("hiddingPosition");
            HSM.SetUpGoto(root.memory, pos, null, "fleeing", false);
        });
    }
}