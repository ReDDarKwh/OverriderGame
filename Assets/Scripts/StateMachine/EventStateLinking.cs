

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.StateMachine
{
    [System.Serializable]
    public class EventStateLinking
    {
        public BaseEvent triggeredOn;
        public string eventName;
        public bool invert = false;
        public EventAction action;
        [System.NonSerialized]
        public EventMessage eventResponse;
    }
}