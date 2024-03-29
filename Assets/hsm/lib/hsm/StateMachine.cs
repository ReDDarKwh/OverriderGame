﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hsm
{

    public class StateMachine
    {
        public State container;
        public List<State> states = new List<State>();
        public State initialState;
        public State currentState;

        private bool eventInProgress = false;

        private Queue<Event> eventQueue = new Queue<Event>();
        private Dictionary<string, State> statesByIds;

        public StateMachine(List<State> pStates)
        {
            states = pStates;
            _setOwners();
            _setInitialState();
        }

        public StateMachine(params State[] pStates)
        {
            states.AddRange(pStates);
            _setOwners();
            _setInitialState();
        }

        private void _setOwners()
        {
            foreach (State state in states)
            {
                state.owner = this;
            }
        }

        private void _setInitialState()
        {
            if (states.Count == 0)
            {
                return;
            }
            initialState = states[0];
        }

        public void Setup()
        {
            Setup(null);
        }

        public void Setup(EventData data)
        {
            if (states.Count == 0)
            {
                throw new UnityException("StateMachine.setup: Must have states!");
            }

            statesByIds = states.ToDictionary(x => x.id, x => x);
            EnterState(null, initialState, data);
        }

        public void TearDown(EventData data)
        {
            if(currentState != null){
                currentState.Exit(currentState, null, data);
                currentState = null;
            }
        }

        public StateMachine AddState(State pState)
        {
            states.Add(pState);
            _setOwners();
            _setInitialState();
            return this;
        }

        public void HandleEvent(string evt)
        {
            HandleEvent(evt, new EventData { });
        }

        public void HandleEvent(string evt, EventData data)
        {
            //if(evt != "update")Debug.Log(evt);
            
            Event myEvent = new Event(evt, data);
            eventQueue.Enqueue(myEvent);

            // process the next event only if no other event is currently processed
            // to not interfere with the run-to-completion model.
            if (eventInProgress)
            {
                return;
            }
            eventInProgress = true;
            Event curEvent;
            while (eventQueue.Count > 0)
            {
                curEvent = eventQueue.Dequeue();
                Handle(curEvent.evt, curEvent.data);
            }
            eventInProgress = false;
        }

        public bool Handle(string evt, EventData data)
        {
            // check if current state is a (nested) statemachine, if so, give it the event.
            // if it handles the event, stop processing here.
            if (currentState is INestedState)
            {
                INestedState nested = currentState as INestedState;
                if (nested.Handle(evt, data))
                {
                    return true;
                }
            }

            if (currentState == null)
            {
                return false;
            }
            if (!currentState.handlers.ContainsKey(evt))
            {
                return false;
            }

            List<Handler> handlers = currentState.handlers[evt];
            foreach (Handler handler in handlers)
            {
                Transition transition = new Transition(currentState, handler);
                if (transition.performTransition(data))
                {
                    return true;
                }
            }
            return false;
        }

        public void SwitchState(State sourceState, State targetState, Action<EventData> action, EventData data)
        {
            currentState.Exit(sourceState, targetState, data);
            if (action != null)
            {
                action.Invoke(data);
            }
            EnterState(sourceState, targetState, data);
        }

        public void EnterState(State sourceState, State targetState, EventData data)
        {
            var targetPath = targetState.owner.GetPath();
            var targetLevel = targetPath.Count;
            var thisLevel = this.GetPath().Count;
            if (targetLevel < thisLevel)
            {
                currentState = initialState;
            }
            else if (targetLevel == thisLevel)
            {
                currentState = targetState;
            }
            else
            {
                currentState = targetPath[thisLevel].container;
            }
            currentState.Enter(sourceState, targetState, data);
        }

        public List<StateMachine> GetPath()
        {
            List<StateMachine> path = new List<StateMachine>();
            StateMachine stateMachine = this;
            while (stateMachine != null)
            {
                path.Insert(0, stateMachine);
                if (stateMachine.container == null)
                {
                    break;
                }
                stateMachine = stateMachine.container.owner;
            }
            return path;
        }

        public List<string> GetActiveStateConfiguration()
        {
            List<string> states = new List<string>();
            states.Add(currentState.id);
            if (currentState is INestedState)
            {
                INestedState nested = currentState as INestedState;
                states.AddRange(nested.getActiveStateConfiguration());
            }
            return states;
        }

        public State GetStateByPath(Stack<string> path){
            var id = path.Pop();
            var state = states.FirstOrDefault(x => x.id == id);

            if(state == null){

            }

            if (state is Sub)
            {
                Sub nested = state as Sub;
                return nested._submachine.GetStateByPath(path);
            } else {
                return state;
            }
        }
    }
}

