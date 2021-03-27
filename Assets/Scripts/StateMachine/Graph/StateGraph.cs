using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Scripts.StateMachine.Graph
{
    [CreateAssetMenu(fileName = "New State Graph", menuName = "xNode Examples/State Graph")]
    public class StateGraph : NodeGraph
    {

        // The current "active" node
        public StateNode current;

        public void Continue()
        {
            current.MoveNext();
        }
    }
}