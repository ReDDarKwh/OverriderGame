using System;
using System.Collections.Generic;
using Bolt;
using Hsm;
using Scripts.Actions;
using Scripts.States;
using UnityEngine;

static class EventRepo
{

    private static HSM GetRoot(EventData data)
    {
        return (HSM)data["root"];
    }

    private static T GetVar<T>(string variableName, EventData data)
    {
        return GetRoot(data).GetComponent<StateMachineMemory>().Get<T>(variableName, false);
    }

    public static Func<EventData, bool> TargetOutOfTargetList = (EventData data) =>
    {
        var target = GetVar<GameObject>("target", data);
        return !data.Root.GetComponent<ChasingState>().chasingAction.dataInputs["Targets"].Contains(target);
    };

    public static Func<EventData, bool> HasTarget = (EventData data) =>
    {
        return GetVar<GameObject>("target", data) != null;
    };

    public static Func<EventData, bool> TargetInTargetList = (EventData data) =>
    {
        return data.Root.GetComponent<ChasingState>().chasingAction.dataInputsHasData["Targets"];
    };

    internal static Func<EventData, bool> Timeout(float v)
    {
        return (EventData data) =>
        {
            return ((Hsm.State)data["state"]).logicState.getStateRunTime() > v;
        };
    }
}