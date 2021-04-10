using System;
using System.Collections;
using System.Collections.Generic;
using Hsm;
using Scripts.States;
using UnityEngine;

public abstract class HSM : MonoBehaviour
{
    protected StateMachine stateMachine;
    private Dictionary<string, object> updateData;

    void Start()
    {
        stateMachine = new StateMachine();
        Init(stateMachine, this);
        stateMachine.Setup();

        updateData = new Dictionary<string, object>();
        updateData.Add("root", this);
    }

    void Update()
    {
        stateMachine.HandleEvent("update", updateData);
    }

    public State AddState(AbstractState state, string name)
    {
        var s = new StateAdapter(state, name).GetState();
        stateMachine.AddState(s);
        return s;
    }

    public abstract void Init(StateMachine sm, HSM root);


}
