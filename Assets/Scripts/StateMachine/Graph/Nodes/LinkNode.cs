using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Scripts.StateMachine.Graph
{
    public class LinkNode : Node
    {
        // Use this for initialization
        [Input(ShowBackingValue.Never, ConnectionType.Multiple)] public List<string> from;
        [Output(ShowBackingValue.Never, ConnectionType.Override)] public List<BaseState> to;

        [HideInInspector]
        public string trigger;
        public string triggerName;
        public bool invert;

        protected override void Init()
        {
            base.Init();
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return this;
        }
    }
}

