using System.Collections;
using System.Collections.Generic;
using Hsm;
using UnityEngine;

public abstract class AbstractEvent
{
    internal string eventName;
    internal StateMachine sm;

    public abstract bool Check();

    public void Execute()
    {
        sm.HandleEvent(eventName);
    }
}
