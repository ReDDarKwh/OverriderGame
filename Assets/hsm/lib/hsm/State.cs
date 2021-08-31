using System;
using System.Collections.Generic;
using Scripts.States;
using UnityEngine;

namespace Hsm
{

    public static class ExtensionMethods
    {
        public static T OnEnter<T>(this T state, Action action) where T : State
        {
            state.enterAction = action;
            state.enterActionWithData = null;
            return state;
        }

        public static T OnEnter<T>(this T state, Action<EventData> action) where T : State
        {
            state.enterActionWithData = action;
            state.enterAction = null;
            return state;
        }

        public static T OnEnter<T>(this T state, Action<State, State, EventData> action) where T : State
        {
            state.enterActionWithStatesAndData = action;
            state.enterActionWithData = null;
            state.enterAction = null;
            return state;
        }

        public static T OnExit<T>(this T state, Action action) where T : State
        {
            state.exitAction = action;
            state.exitActionWithData = null;
            return state;
        }

        public static T OnExit<T>(this T state, Action<EventData> action) where T : State
        {
            state.exitActionWithData = action;
            state.exitAction = null;
            return state;
        }

        public static T OnExit<T>(this T state, Action<State, State, EventData> action) where T : State
        {
            state.exitActionWithStatesAndData = action;
            state.exitActionWithData = null;
            state.exitAction = null;
            return state;
        }

        public static T AddHandler<T>(this T state, string eventName, State target) where T : State
        {
            state.createHandler(eventName, target, TransitionKind.External, null, null);
            return state;
        }

        public static T AddHandler<T>(this T state, string eventName, State target, TransitionKind kind) where T : State
        {
            state.createHandler(eventName, target, kind, null, null);
            return state;
        }

        public static T AddHandler<T>(this T state, string eventName, State target, Action<EventData> action) where T : State
        {
            state.createHandler(eventName, target, TransitionKind.External, action, null);
            return state;
        }

        public static T AddHandler<T>(this T state, string eventName, State target, TransitionKind kind, Action<EventData> action) where T : State
        {
            state.createHandler(eventName, target, kind, action, null);
            return state;
        }

        public static T AddHandler<T>(this T state, string eventName, State target, Func<EventData, bool> guard) where T : State
        {
            state.createHandler(eventName, target, TransitionKind.External, null, guard);
            return state;
        }

        public static T AddUpdateHandler<T>(this T state, State target, Func<EventData, bool> guard, Action<EventData> action = null) where T : State
        {
            state.createHandler("update", target, TransitionKind.External, action, guard);
            return state;
        }

        public static T AddEnterHandler<T>(this T state, State target, Func<EventData, bool> guard, Action<EventData> action = null) where T : State
        {
            state.createHandler("enter", target, TransitionKind.External, action, guard);
            return state;
        }

        public static T AddHandler<T>(this T state, string eventName, State target, TransitionKind kind, Func<EventData, bool> guard) where T : State
        {
            state.createHandler(eventName, target, kind, null, guard);
            return state;
        }

        public static T AddHandler<T>(this T state, string eventName, State target, TransitionKind kind, Action<EventData> action, Func<EventData, bool> guard) where T : State
        {
            state.createHandler(eventName, target, kind, action, guard);
            return state;
        }
    }

    public class State
    {
        [SerializeField]
        public string id;
        public StateMachine owner;
        public Action enterAction = null;
        public Action<EventData> enterActionWithData = null;
        public Action<State, State, EventData> enterActionWithStatesAndData = null;
        public Action exitAction = null;
        public Action<EventData> exitActionWithData = null;
        public Action<State, State, EventData> exitActionWithStatesAndData = null;
        public Dictionary<string, List<Handler>> handlers = new Dictionary<string, List<Handler>>();
        public float enterTime;

        public AbstractState logicState { get; internal set; }

        public State(string pId)
        {
            id = pId;
        }

        public virtual void Enter(State sourceState, State targetstate, EventData data)
        {
            enterTime = Time.time;
            if (enterAction != null)
            {
                enterAction.Invoke();
            }
            if (enterActionWithData != null)
            {
                enterActionWithData.Invoke(data);
            }
            if (enterActionWithStatesAndData != null)
            {
                enterActionWithStatesAndData.Invoke(sourceState, targetstate, data);
            }
        }

        public virtual void Exit(State sourceState, State targetstate, EventData data)
        {
            if (exitAction != null)
            {
                exitAction.Invoke();
            }
            if (exitActionWithData != null)
            {
                exitActionWithData.Invoke(data);
            }
            if (exitActionWithStatesAndData != null)
            {
                exitActionWithStatesAndData.Invoke(sourceState, targetstate, data);
            }
        }

        public void createHandler(string eventName, State target, TransitionKind kind, Action<EventData> action, Func<EventData, bool> guard)
        {
            Handler handler = new Handler(target, kind, action, guard);
            if (!handlers.ContainsKey(eventName))
            {
                handlers[eventName] = new List<Handler>();
            }
            handlers[eventName].Add(handler);
        }

        public bool hasAncestor(State other)
        {
            if (owner.container == null)
            {
                return false;
            }
            if (owner.container == other)
            {
                return true;
            }
            return owner.container.hasAncestor(other);
        }

        public bool hasAncestorStateMachine(StateMachine stateMachine)
        {
            for (var i = 0; i < owner.GetPath().Count; ++i)
            {
                if (owner.GetPath()[i] == stateMachine)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
