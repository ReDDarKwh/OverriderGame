using System;
using System.Collections.Generic;
using Bolt;
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
        return (T)Variables.Object(GetRoot(data).gameObject).Get(variableName);
    }

    public static readonly Func<Dictionary<string, object>, bool> targetOutOfTargetList = (Dictionary<string, object> data) =>
    {
        var target = GetVar<GameObject>("target", data);
        return !GetVar<ExternalLogicAction>("chasingAction", data).dataInputs["Targets"].Contains(target);
    };

    public static readonly Func<Dictionary<string, object>, bool> targetInTargetList = (Dictionary<string, object> data) =>
    {
        return GetVar<ExternalLogicAction>("chasingAction", data).dataInputsHasData["Targets"];
    };
}