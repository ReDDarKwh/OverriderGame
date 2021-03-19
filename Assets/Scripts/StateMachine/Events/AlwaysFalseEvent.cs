﻿using UnityEngine;

namespace Scripts.StateMachine.Events
{
    public class AlwaysFalseEvent : BaseEvent
    {
        public override bool Check(GameObject character, ActiveLinking activeLinking, out EventMessage message)
        {
            message = EventMessage.EmptyMessage;
            return false;
        }

        public override void Init(ActiveLinking activeLinking)
        {
            continousCheck = false;
        }
    }
}


