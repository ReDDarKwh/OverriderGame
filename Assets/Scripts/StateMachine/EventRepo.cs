using System;
using System.Collections.Generic;
using Hsm;
using Scripts.Actions;
using Scripts.States;
using UnityEngine;
using System.Linq;

static class EventRepo
{
    public static Func<EventData, bool> TargetOutOfTargetList = (EventData data) =>
    {
        var target = data.Memory.Get<GameObject>("target", false);
        return !data.Root.GetComponent<ChasingState>().chasingAction.dataInputs["Targets"].Contains(target);
    };

    public static Func<EventData, bool> HasTarget = (EventData data) =>
    {
        return data.Memory.Get<GameObject>("target", false) != null;
    };

    public static Func<EventData, bool> TargetInTargetList = (EventData data) =>
    {
        return data.Root.GetComponent<ChasingState>().chasingAction.dataInputsHasData["Targets"];
    };

    internal static Func<EventData, bool> Timeout(float v)
    {
        return (EventData data) =>
        {
            var t = Time.time;
            return t - data.GetVar<Hsm.State>("state").enterTime > v;
        };
    }

    internal static Func<EventData, bool> Concat(params Func<EventData, bool>[] guards)
    {
        return (EventData data) =>
        {
            return guards.All(x => x.Invoke(data));
        };
    }
}