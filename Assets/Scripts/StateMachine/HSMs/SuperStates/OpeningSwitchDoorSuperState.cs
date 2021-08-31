using Hsm;
using Scripts.States;
using UnityEngine;

class OpeningSwitchDoorSuperState : SuperState
{

    public OpeningSwitchDoorSuperState(StateMachine sm, HSM root, string name) : base(sm, root, name)
    {
    }

    public override void Init(StateMachine sm, HSM root)
    {
        var start = AddState(root.GetComponent<EmptyState>(), "start");
        var go = AddState(root.GetComponent<GotoState>(), "gotoSwitch");
        var interacting = AddState(root.GetComponent<InteractingState>(), "interacting");

        start.AddEnterHandler(go, null, (EventData data) =>
        {
            Debug.Log("opening door");
            HSM.SetUpGoto(root.memory, null,
            root.memory.Get<GameObject>("switch", false).transform, 
            "switch", true, "isAtSwitch");
        });

        go.AddHandler("isAtSwitch", interacting, TransitionKind.External, 
        (EventData data) => {
            root.memory.Set("interactableName", "switch");
        });
    }
}