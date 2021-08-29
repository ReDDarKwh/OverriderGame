using System.Collections.Generic;
using Hsm;
using Scripts.States;
using UnityEngine;

class GotoSuperState : SuperState
{

    public GotoSuperState(StateMachine sm, HSM root, string name) : base(sm, root, name)
    {
    }

    public override void Init(StateMachine sm, HSM root)
    {
        var go = AddState(root.GetComponent<GotoState>(), "goto");
        var interactingWithDoorSwitch = new OpeningSwitchDoorSuperState(sm, root, "interactingWithDoorSwitch");
        var cleanUp = AddState(root.GetComponent<EmptyState>(), "cleanUp");

        go.AddHandler("activateSwitchRequested", interactingWithDoorSwitch.sub, TransitionKind.External,
        (EventData data) =>
        {
            var s = data.GetVar<GameObject>("switch");
            data.Memory.Set("switch", s);
            data.Memory.Set("doorController", HSM.GetVar<DoorController>("doorController", data));

            data.Memory.Set("oldGotoSettingsName", data.Memory.Get<string>("gotoSettingsName"));
            data.Memory.Set("oldTargetTransform", data.Memory.Get<Transform>("targetTransform"));
            data.Memory.Set("oldLookAtTarget", data.Memory.Get<bool>("lookAtTarget"));
            data.Memory.Set("oldPositionEventName", data.Memory.Get<string>("positionEventName"));
        },
        (EventData data) =>
        {
            var doorController = data.GetVar<DoorController>("doorController");
            if (!doorController.openingAction.outputGate.currentValue)
            {
                var pathFindingNav = data.Root.GetComponent<PathFindingNav>();
                return pathFindingNav.IsGoingThroughDoor(doorController);
            }
            return false;
        });

        interactingWithDoorSwitch.sub.AddHandler("interactionDone", go, TransitionKind.External, (EventData data) =>
        {
            RestorePreviousGoto(data);
        });

        interactingWithDoorSwitch.sub.AddUpdateHandler(go, (EventData data) =>
        {
            return data.Memory.Get<DoorController>("doorController", false).openingAction.outputGate.currentValue;
        }, (EventData data) =>
        {
            RestorePreviousGoto(data);
        });

        go.AddHandler("cleanUpGoto", cleanUp, TransitionKind.External, (EventData data) => {
            data.Memory.Delete("doorController");
            data.Root.TriggerEvent("isAtPosition");
        });
    }

    private static void RestorePreviousGoto(EventData data)
    {
        HSM.SetUpGoto(data.Memory, null,
                    data.Memory.Get<Transform>("oldTargetTransform"),
                    data.Memory.Get<string>("oldGotoSettingsName"),
                    data.Memory.Get<bool>("oldLookAtTarget"),
                    data.Memory.Get<string>("oldPositionEventName"));
    }
}