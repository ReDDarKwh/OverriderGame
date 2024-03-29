﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsm
{

    public class Transition
    {

        private State sourceState;
        private State targetState;
        private TransitionKind kind;
        private Action<EventData> action;
        private Func<EventData, bool> guard;

        public Transition(State sourceState, Handler handler)
        {
            this.sourceState = sourceState;
            this.targetState = handler.targetState;
            this.kind = handler.kind;
            this.action = handler.action;
            this.guard = handler.guard;
        }

        public bool performTransition(EventData data)
        {

            data["state"] = sourceState;

            if (!_canPerformTransition(data))
            {
                return false;
            }
            if (kind == TransitionKind.Internal)
            {
                return _performInternalTransition(data);
            }
            else if (kind == TransitionKind.Local)
            {
                return _performLocalTransition(data);
            }
            else
            {
                return _performExternalTransition(data);
            }
        }

        private bool _performInternalTransition(EventData data)
        {
            if (action != null)
            {
                action.Invoke(data);
            }
            return true;
        }

        private bool _performLocalTransition(EventData data)
        {
            if (targetState == null)
            {
                return false;
            }
            if (!sourceState.hasAncestor(targetState) && !targetState.hasAncestor(sourceState))
            {
                return false;
            }
            StateMachine lca = _findLeastCommonAncestor();
            Sub containingSubState = lca.currentState as Sub;
            lca = containingSubState._submachine;
            lca.SwitchState(sourceState, targetState, action, data);
            return true;
        }

        private bool _performExternalTransition(EventData data)
        {
            if (targetState == null)
            {
                return false;
            }
            StateMachine lca = _findLeastCommonAncestor();
            lca.SwitchState(sourceState, targetState, action, data);
            return true;
        }

        private bool _canPerformTransition(EventData data)
        {
            return (guard == null || guard.Invoke(data));
        }

        private StateMachine _findLeastCommonAncestor()
        {
            List<StateMachine> sourcePath = sourceState.owner.GetPath();
            List<StateMachine> targetPath = targetState.owner.GetPath();

            StateMachine lca = null;
            for (var i = sourcePath.Count - 1; i >= 0; i--)
            {
                StateMachine stateMachine = sourcePath[i];
                if (targetPath.Contains(stateMachine))
                {
                    lca = stateMachine;
                    break;
                }
            }
            return lca;
        }
    }

}
