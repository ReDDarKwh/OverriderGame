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
    private EventData baseData;
    public List<string> currentState;
    internal StateMachineMemory memory;

    public static T GetVar<T>(string variableName, EventData data)
    {
        object result;
        var found = data.TryGetValue(variableName, out result);
        return found ? (T)result : default(T);
    }
    public static HSM GetRoot(EventData data)
    {
        return GetVar<HSM>("root", data);
    }

    public static void SetUpGoto(StateMachineMemory memory, Vector3? targetPos, Transform targetTransform, string gotoSettingsName, bool lookAtTarget, string positionEventName = "cleanUpGoto")
    {
        if (targetPos != null)
        {
            memory.Set("targetPos", targetPos.Value);
        }
        memory.Set("targetTransform", targetTransform);
        memory.Set("gotoSettingsName", gotoSettingsName);
        memory.Set("lookAtTarget", lookAtTarget);
        memory.Set("positionEventName", positionEventName);
    }

    void Start()
    {
        stateMachine = new StateMachine();
        Init(stateMachine, this);
        stateMachine.Setup();
        memory = GetComponent<StateMachineMemory>();
    }

    private EventData GetBaseData(){
        if(baseData == null){
            baseData = new EventData{{"root" , this}};
        }
        return baseData;  
    }

    void Update()
    {
        stateMachine.HandleEvent("update", GetBaseData());
        currentState = stateMachine.GetActiveStateConfiguration();
    }

    public void TriggerEvent(string evtName, EventData data)
    {
        stateMachine.HandleEvent(evtName, 
            new EventData(data.Concat(GetBaseData()).ToDictionary(x => x.Key, x => x.Value))
        );
    }
    public void TriggerEvent(string evtName)
    {
        stateMachine.HandleEvent(evtName, GetBaseData());
    }
    public State AddState(AbstractState state, string name)
    {
        var s = new StateAdapter(state, name).GetState();
        stateMachine.AddState(s);
        return s;
    }
    public abstract void Init(StateMachine sm, HSM root);
}
