using System.Collections.Generic;
using Hsm;
using Scripts.States;
using UnityEngine;

public class StateAdapter
{
    private State state;

    public StateAdapter(AbstractState abstractState, string name)
    {
        state = new State(name);
        state.logicState = abstractState;

        state.OnEnter((source, target, data) =>
        {
            abstractState.PreEnterState();
        });

        state.OnExit((source, target, data) =>
        {
            abstractState.PreExitState();
        });
    }

    public State GetState()
    {
        return state;
    }
}
