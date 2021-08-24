using System.Collections.Generic;
using UnityEngine;

namespace Scripts.States
{
    public class InstantState : AbstractState
    {
        public override void StateEnter(Dictionary<string, object> evtData)
        {
            root.TriggerEvent("done");
        }

        public override void StateExit()
        {
        }

        public override void StateUpdate()
        {
        }
    }
}



