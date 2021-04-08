using System;
using System.Collections;
using System.Collections.Generic;
using Hsm;
using Scripts.States;
using UnityEngine;

public abstract class HierarchicalStateMachine : MonoBehaviour
{
    protected StateMachine stateMachine;
    protected HashSet<string> continuousCheckEvents = new HashSet<string>();

    void Start()
    {
        stateMachine = new StateMachine();
        stateMachine.AddState(Init(stateMachine));
        stateMachine.Setup();
    }

    void Update()
    {
        foreach (var handler in stateMachine.currentState.handlers.Keys)
        {
            if (continuousCheckEvents.Contains(handler))
            {
                stateMachine.HandleEvent(handler);
            }
        }
    }

    public abstract Sub Init(StateMachine sm);

    protected void AddEvent(State fromState, State toState, string eventName, bool continuous)
    {
        fromState.AddHandler(eventName, toState);

        if (continuous)
        {
            continuousCheckEvents.Add(eventName);
        }
    }

    protected State AddState(AbstractState state, string name)
    {
        var s = new StateAdapter(state, name).GetState();
        stateMachine.AddState(s);
        return s;
    }
}
