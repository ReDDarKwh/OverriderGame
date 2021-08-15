using System;
using System.Collections.Generic;
using Bolt;
using Hsm;
using Scripts.Actions;
using UnityEngine;

static class EventRepo
{

    private static HSM GetRoot(Dictionary<string, object> data)
    {
        return (HSM)data["root"];
    }

    private static T GetVar<T>(string variableName, Dictionary<string, object> data)
    {
        return GetRoot(data).GetComponent<StateMachineMemory>().Get<T>(variableName, false);
    }

    public static Func<Dictionary<string, object>, bool> TargetOutOfTargetList = (Dictionary<string, object> data) =>
    {
        var target = GetVar<GameObject>("target", data);
        return !GetVar<ExternalLogicAction>("chasingAction", data).dataInputs["Targets"].Contains(target);
    };

    public static Func<Dictionary<string, object>, bool> HasTarget = (Dictionary<string, object> data) =>
    {
        return GetVar<GameObject>("target", data) != null;
    };

    public static Func<Dictionary<string, object>, bool> TargetInTargetList = (Dictionary<string, object> data) =>
    {
        return GetVar<ExternalLogicAction>("chasingAction", data).dataInputsHasData["Targets"];
    };

    internal static Func<Dictionary<string, object>, bool> Timeout(float v)
    {
        return (Dictionary<string, object> data) =>
        {
            return ((Hsm.State)data["state"]).logicState.getStateRunTime() > v;
        };
    }
}