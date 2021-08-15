using System.Collections.Generic;
using Hsm;
using Scripts.States;
using UnityEngine;

public class StateAdapter
{
    private State state;

    public StateAdapter(AbstractState state, string name)
    {
        this.state = new State(name);

        this.state.logicState = state;

        this.state.OnEnter((source, target, data) =>
        {
            state.isRunning = true;
            state.StateEnter(data);
            state.enterTime = Time.time;
            this.state.owner.HandleEvent("enter", 
                new Dictionary<string, object>(){{"root", state.GetComponent<HSM>()}}
                );
        });
        this.state.OnExit((source, target, data) =>
        {
            state.isRunning = false;
            state.StateExit();
        });
    }

    public State GetState()
    {
        return state;
    }
}
