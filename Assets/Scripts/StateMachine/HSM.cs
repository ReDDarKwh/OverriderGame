using System;
using System.Collections;
using System.Collections.Generic;
using Hsm;
using Scripts.States;
using UnityEngine;
using System.Linq;

public abstract class HSM : MonoBehaviour
{
    protected StateMachine stateMachine;
    private Dictionary<string, object> baseData;
    public List<string> currentState;
    internal StateMachineMemory memory;

    public static T GetVar<T>(string variableName, Dictionary<string, object> data)
    {
        object result;
        var found = data.TryGetValue(variableName, out result);
        return found ? (T)result : default(T);
    }
    public static HSM GetRoot(Dictionary<string, object> data)
    {
        return GetVar<HSM>("root", data);
    }

    public static void SetUpGoto(StateMachineMemory memory, Vector3? targetPos, Transform targetTransform, string gotoSettingsName, bool lookAtTarget)
    {
        if (targetPos != null)
        {
            memory.Set("targetPos", targetPos.Value);
        }
        memory.Set("targetTransform", targetTransform);
        memory.Set("gotoSettingsName", gotoSettingsName);
        memory.Set("lookAtTarget", lookAtTarget);
    }

    void Start()
    {
        stateMachine = new StateMachine();
        Init(stateMachine, this);
        stateMachine.Setup();

        baseData = new Dictionary<string, object>();
        baseData.Add("root", this);

        memory = GetComponent<StateMachineMemory>();
    }
    void Update()
    {
        stateMachine.HandleEvent("update", baseData);

        currentState = stateMachine.GetActiveStateConfiguration();
    }
    public void TriggerEvent(string evtName, Dictionary<string, object> data)
    {
        stateMachine.HandleEvent(evtName, 
            data.Concat(baseData).ToDictionary(x => x.Key, x => x.Value)
        );
    }
    public void TriggerEvent(string evtName)
    {
        stateMachine.HandleEvent(evtName, baseData);
    }
    public State AddState(AbstractState state, string name)
    {
        var s = new StateAdapter(state, name).GetState();
        stateMachine.AddState(s);
        return s;
    }
    public abstract void Init(StateMachine sm, HSM root);
}
