﻿using UnityEngine;

namespace Scripts.StateMachine
{
    public abstract class BaseEvent : ScriptableObject
    {
        public bool continousCheck = true;
        public abstract bool Check(GameObject gameObject, ActiveLinking activeLinking, out EventMessage message);
        public abstract void Init(ActiveLinking activeLinking);
    }
}


