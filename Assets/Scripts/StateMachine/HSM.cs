using System;
using System.Collections;
using System.Collections.Generic;
using Hsm;
using Scripts.States;
using UnityEngine;
using System.Linq;
using Lowscope.Saving;
using Newtonsoft.Json;

public abstract class HSM : MonoBehaviour, ISaveable
{
    protected StateMachine stateMachine;
    private EventData baseData;
    public List<string> currentState;
    internal StateMachineMemory memory;
    private EnemySharedInfoManager sharedInfoManager;

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
            memory.Set("targetPos", targetPos.Value, MemoryType.Value);
        }
        memory.Set("targetTransform", targetTransform, MemoryType.Component);
        memory.Set("gotoSettingsName", gotoSettingsName, MemoryType.Value);
        memory.Set("lookAtTarget", lookAtTarget, MemoryType.Value);
        memory.Set("positionEventName", positionEventName, MemoryType.Value);
    }

    void Start()
    {
        memory = GetComponent<StateMachineMemory>();
        stateMachine = new StateMachine();
        Init(stateMachine, this);
        stateMachine.Setup();

        sharedInfoManager = GameObject.FindGameObjectWithTag("SceneManager")
        .GetComponent<EnemySharedInfoManager>();
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

    public string OnSave()
    {
        var s = new SavedHSM{path = stateMachine.GetActiveStateConfiguration(), memory = memory.OnSave()};
        return JsonConvert.SerializeObject(s);
    }

    public void OnLoad(string data)
    {
        var s = JsonConvert.DeserializeObject<SavedHSM>(data);
        var path = new Stack<string>(s.path.ToArray().Reverse());
        var state = stateMachine.GetStateByPath(path);
        memory.OnLoad(s.memory);
        stateMachine.TearDown(GetBaseData());
        stateMachine.EnterState(null, state, GetBaseData());
    }

    public bool OnSaveCondition()
    {
        return this.gameObject.activeSelf && memory != null && stateMachine != null;
    }
}
