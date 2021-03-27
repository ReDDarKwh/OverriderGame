using System.Collections;
using System.Collections.Generic;
using Scripts.StateMachine.Graph;
using UnityEngine;


namespace Scripts.StateMachine
{
    [System.Serializable]
    public enum EventActionType
    {
        TRANSITION,
        NONE,
        REMOVE,
        ADD,
        ADD_OVERRIDE,
        TRANSITION_OVERRIDE
    }

    [System.Serializable]
    public class EventAction
    {
        [System.NonSerialized]
        public StateNode nextState;
        [System.NonSerialized]
        public BaseState fromState;
    }
}


