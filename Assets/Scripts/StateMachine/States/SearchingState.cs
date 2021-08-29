
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
            memory.Set("unsureTime", unsureTime);
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