using System.Collections.Generic;
using Hsm;
using Scripts.States;
using UnityEngine;

class InvestigatingNoiseSuperState : SuperState
{

    public InvestigatingNoiseSuperState(StateMachine sm, HSM root, string name) : base(sm, root, name)
    {
    }

    public override void Init(StateMachine sm, HSM root)
    {
        var start = AddState(root.GetComponent<IntriguedState>(), "start");
        var go = new GotoSuperState(sm, root, "goto");
        var lookingAround = AddState(root.GetComponent<LookAroundState>(), "lookingAround");
        
        start.AddUpdateHandler(go.sub, EventRepo.Timeout(1), (EventData data) => {
            var memory = HSM.GetRoot(data).memory;
            var pos = memory.Get<Vector3>("targetPos", false);
            HSM.SetUpGoto(
                memory,
                pos,
                null,
                "investigatingNoise",
                false
            );
        });

        go.sub.AddHandler("isAtPosition", lookingAround);
        go.sub.AddHandler("isStuck", lookingAround);
        lookingAround.AddHandler("noiseHeard", start, TransitionKind.External, (EventData data) =>
        {
            var memory = HSM.GetRoot(data).memory;
            var pos = HSM.GetVar<Vector3>("subject", data);
            memory.Set("targetPos", pos, MemoryType.Value);
        });
    }
}