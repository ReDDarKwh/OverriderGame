using System;
using System.Collections;
using System.Collections.Generic;
using Hsm;
using Scripts.States;
using UnityEngine;

public abstract class SuperState
{
    public Sub sub;

    public SuperState(StateMachine sm, HSM root, string superStateName)
    {
        StateMachine theSubmachine = new StateMachine();
        sub = new Sub(superStateName, theSubmachine);
        Init(theSubmachine, root);
        sm.AddState(sub);
    }

    public abstract void Init(StateMachine sm, HSM root);

    public State AddState(AbstractState state, string name)
    {
        var s = new StateAdapter(state, name).GetState();
        sub._submachine.AddState(s);
        return s;
    }
}
