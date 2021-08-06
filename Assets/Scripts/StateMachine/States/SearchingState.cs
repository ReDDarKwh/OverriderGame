
using System.Collections.Generic;
using Scripts.States;
using UnityEngine;


namespace Scripts.States
{
    public class SearchingState : AbstractState
    {
        public override void StateEnter(Dictionary<string, object> evtData)
        {
            memory.Set("gotoSettingsName", "searching");
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