﻿using System.Collections.Generic;

namespace Hsm
{

    public class Sub : State, INestedState
    {

        public StateMachine _submachine;

        public Sub(string theId, StateMachine theSubmachine) : base(theId)
        {
            _submachine = theSubmachine;
            _submachine.container = this;
        }

        public bool Handle(string evt, EventData data)
        {
            return _submachine.Handle(evt, data);
        }

        public override void Enter(State sourceState, State targetstate, EventData data)
        {
            base.Enter(sourceState, targetstate, data);
            _submachine.EnterState(sourceState, targetstate, data);
        }

        public override void Exit(State sourceState, State targetstate, EventData data)
        {
            _submachine.TearDown(data);
            base.Exit(sourceState, targetstate, data);
        }

        public List<string> getActiveStateConfiguration()
        {
            return _submachine.GetActiveStateConfiguration();
        }
    }

}
