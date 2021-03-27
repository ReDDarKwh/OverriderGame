using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.StateMachine.Graph;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.StateMachine
{
    public partial class StateMachine : MonoBehaviour
    {
        private Dictionary<BaseState, List<StateMachineEventWithActiveLinking>> UpdateSubs =
            new Dictionary<BaseState, List<StateMachineEventWithActiveLinking>>();
        private Dictionary<BaseState, List<UnityEvent>> LeaveSubs =
            new Dictionary<BaseState, List<UnityEvent>>();
        private Dictionary<BaseState, List<StateMachineEventWithActiveLinking>> EnterSubs =
            new Dictionary<BaseState, List<StateMachineEventWithActiveLinking>>();
        private ActiveLinking activeLinking;
        public BaseState entryState;
        public StateGraph stateMachineGraph;
        public bool debugShowStates = false;

        public void Start()
        {
        }

        public void Update()
        {
            if (activeLinking == null)
            {
                StartState(stateMachineGraph.current, null, false);
            }

            activeLinking.state.StateUpdate(this, activeLinking);
            AfterUpdate(activeLinking);

            foreach (var link in activeLinking.links)
            {
                EventMessage message;

                if (link.triggeredOn.continousCheck &&
                    (link.invert ?
                        !link.triggeredOn.Check(gameObject, activeLinking, out message) :
                        link.triggeredOn.Check(gameObject, activeLinking, out message)
                    ))
                {
                    link.eventResponse = message;
                    ExecuteEventAction(activeLinking.state, link);
                    break;
                }
            }
        }

        public void AddSubscription(StateMachineSubscription subscription)
        {
            if (subscription.onStateEnter != null)
            {
                if (!EnterSubs.ContainsKey(subscription.state))
                {
                    EnterSubs[subscription.state] = new List<StateMachineEventWithActiveLinking>();
                }
                EnterSubs[subscription.state].Add(subscription.onStateEnter);
            }
            if (subscription.onStateUpdate != null)
            {
                if (!UpdateSubs.ContainsKey(subscription.state))
                {
                    UpdateSubs[subscription.state] = new List<StateMachineEventWithActiveLinking>();
                }
                UpdateSubs[subscription.state].Add(subscription.onStateUpdate);
            }
            if (subscription.onStateLeave != null)
            {
                if (!LeaveSubs.ContainsKey(subscription.state))
                {
                    LeaveSubs[subscription.state] = new List<UnityEvent>();
                }
                LeaveSubs[subscription.state].Add(subscription.onStateLeave);
            }
        }

        public void AfterUpdate(ActiveLinking linking)
        {
            if (!UpdateSubs.ContainsKey(linking.state))
                return;
            foreach (var sub in UpdateSubs[linking.state])
            {
                sub.Invoke(linking);
            }
        }

        public void AfterEnter(ActiveLinking linking)
        {
            if (!EnterSubs.ContainsKey(linking.state))
                return;
            foreach (var sub in EnterSubs[linking.state])
            {
                sub.Invoke(linking);
            }
        }
        public void AfterLeave(BaseState state)
        {
            if (!LeaveSubs.ContainsKey(state))
                return;
            foreach (var sub in LeaveSubs[state])
            {
                sub.Invoke();
            }
        }

        // find the events with the same name in the active states and trigger them
        public void TriggerEvent(string eventName, EventMessage message)
        {
            var pendingActions = new Queue<Tuple<BaseState, EventStateLinking>>();

            if (activeLinking != null && activeLinking.linksByEventName.ContainsKey(eventName))
            {
                ExecuteEventAction(activeLinking.state, activeLinking.linksByEventName[eventName]);
            }
        }

        private void ExecuteEventAction(BaseState fromState, EventStateLinking e)
        {
            EndState(fromState);
            if (e.action.nextState != null)
            {
                StartState(e.action.nextState, e, true);
            }
        }

        private class LinkRepo
        {
            public BaseState startState;
            public Dictionary<string, IEnumerable<EventStateLinking>> linksByState { get; set; }
        }

        private class EventLinkingGroups
        {
            public IEnumerable<BaseState> states { get; set; }
            public IEnumerable<string> tagnames { get; set; }
            public string triggerName { get; set; }
            public EventActionType actionType { get; set; }
        }

        private IEnumerable<EventStateLinking> GenerateLinkRepo(StateNode stateNode)
        {
            return null;
        }

        public void StartState(StateNode stateToSwitchTo, EventStateLinking e, bool doAnOverride)
        {

            var state = (BaseState)GetComponent(stateToSwitchTo.state);

            if (activeLinking.state == state)
            {
                if (doAnOverride)
                {
                    EndState(activeLinking.state);
                }
                else
                {
                    return;
                }
            }

            var links = stateToSwitchTo
                .GetOutputPort("exit")
                .GetConnections()
                .Select(x => x.node as LinkNode)
                .Select(x => new EventStateLinking
                {
                    triggeredOn = (BaseEvent)GetComponent(x.trigger),
                    eventName = x.triggerName,
                    invert = x.invert,
                    action = new EventAction
                    {
                        nextState = ((StateNode)(x.GetOutputPort("to").GetConnection(0).node))
                    }
                });

            activeLinking = new ActiveLinking()
            {
                links = links,
                linksByEventName = links.ToDictionary(k => k.eventName, v => v),
                linkingProperties = new Dictionary<string, object>(),
                state = state
            };

            state.Enter(this, e?.eventResponse, activeLinking);
            AfterEnter(activeLinking);

            foreach (var link in activeLinking.links)
            {
                link.triggeredOn.Init(activeLinking);
            }
        }

        public void EndState(BaseState state)
        {
            if (activeLinking.state == state)
            {
                activeLinking = null;
                state.Leave(this);
                AfterLeave(state);
            }
        }
    }
}