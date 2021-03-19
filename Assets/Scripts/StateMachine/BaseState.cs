
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.StateMachine
{
    public abstract class BaseState : MonoBehaviour
    {
        public List<string> tags = new List<string>() { "global" };
        public string stateName;

        public HashSet<string> GetTagsPlusName()
        {
            var hashCopy = new HashSet<string>(tags);
            hashCopy.Add(stateName);
            return hashCopy;
        }

        public abstract void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking);

        public abstract void Enter(StateMachine stateMachine, EventMessage eventResponse, ActiveLinking activeLinking);

        public abstract void Leave(StateMachine stateMachine);
    }

}

