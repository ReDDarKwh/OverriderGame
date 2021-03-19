using System.Collections;
using System.Collections.Generic;
using Scripts.Actions;
using UnityEngine;

namespace Scripts.StateMachine.States
{
    public class EmptyState : BaseState
    {
        void Awake()
        {
            stateName = "EmptyState";
        }

        public override void Enter(StateMachine stateMachine, EventMessage eventResponse, ActiveLinking activeLinking)
        {
        }

        public override void Leave(StateMachine stateMachine)
        {
        }

        public override void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking)
        {
        }
    }
}

