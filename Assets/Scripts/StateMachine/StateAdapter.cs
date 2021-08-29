using System.Collections.Generic;
using Hsm;
using Scripts.States;
using UnityEngine;

public class StateAdapter
{
    private State state;

    public StateAdapter(AbstractState abstractState, string name)
    {
        this.state = new State(name);

        this.state.logicState = abstractState;

        this.state.OnEnter((source, target, data) =>
        {
            abstractState.enterTime = Time.time;
            abstractState.isActive = true;
        });
        this.state.OnExit((source, target, data) =>
        {
            abstractState.isActive = false;
        });
    }

    public State GetState()
    {
        return state;
    }
}
