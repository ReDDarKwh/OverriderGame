
using System.Collections.Generic;
using Scripts.States;
using UnityEngine;


namespace Scripts.States
{
    public class SearchingState : AbstractState
    {
        public float unsureTime;
        public override void StateEnter()
        {
            memory.Set("unsureTime", unsureTime, MemoryType.Value);
            memory.Set("gotoSettingsName", "searching", MemoryType.Value);
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