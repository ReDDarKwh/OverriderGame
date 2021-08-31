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
        var interactionBuffer = AddState(root.GetComponent<EmptyState>(), "interactionBuffer");
        var cleanUp = AddState(root.GetComponent<EmptyState>(), "cleanUp");

        go.AddHandler("activateSwitchRequested", interactingWithDoorSwitch.sub, TransitionKind.External,
        (EventData data) =>
        {
            var s = data.GetVar<GameObject>("switch");
            root.memory.Set("switch", s);
            root.memory.Set("doorController", HSM.GetVar<DoorController>("doorController", data));

            root.memory.Set("oldGotoSettingsName", root.memory.Get<string>("gotoSettingsName"));
            root.memory.Set("oldTargetPos", root.memory.Get<Vector3?>("targetPos"));
            root.memory.Set("oldTargetTransform", root.memory.Get<Transform>("targetTransform"));
            root.memory.Set("oldLookAtTarget", root.memory.Get<bool>("lookAtTarget"));
            root.memory.Set("oldPositionEventName", root.memory.Get<string>("positionEventName"));
        },
        (EventData data) =>
        {
            var doorController = data.GetVar<DoorController>("doorController");
            if (!doorController.openingAction.outputGate.currentValue)
            {
                var pathFindingNav = root.GetComponent<PathFindingNav>();
                return pathFindingNav.IsGoingThroughDoor(doorController);
            }
            return false;
        });

        interactingWithDoorSwitch.sub.AddHandler("interactionDone", interactionBuffer, TransitionKind.External, (EventData data) =>
        {
            RestorePreviousGoto(data);
        });

        interactionBuffer.AddUpdateHandler(go, EventRepo.Timeout(0.5f));

        interactingWithDoorSwitch.sub.AddUpdateHandler(go, (EventData data) =>
        {
            return root.memory.Get<DoorController>("doorController", false).openingAction.outputGate.currentValue;
        }, (EventData data) =>
        {
            RestorePreviousGoto(data);
        });

        go.AddHandler("cleanUpGoto", cleanUp, TransitionKind.External, (EventData data) =>
        {
            CleanUp(data);
            root.TriggerEvent("isAtPosition");
        });

        go.AddHandler("isUnreachable", cleanUp, TransitionKind.External, (EventData data) => {
            CleanUp(data);
            root.TriggerEvent("isStuck");
        });
    }

    private static void CleanUp(EventData data)
    {
        data.Memory.Delete("targetPos");
        data.Memory.Delete("targetTransform");
        data.Memory.Delete("doorController");
    }

    private static void RestorePreviousGoto(EventData data)
    {
        HSM.SetUpGoto(data.Memory, 
                    data.Memory.Get<Vector3?>("oldTargetPos"),
                    data.Memory.Get<Transform>("oldTargetTransform"),
                    data.Memory.Get<string>("oldGotoSettingsName"),
                    data.Memory.Get<bool>("oldLookAtTarget"),
                    data.Memory.Get<string>("oldPositionEventName"));
    }
}