using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Scripts.States
{
    public class IntriguedState : AbstractState
    {
        public Creature creature;
        
        public override void StateEnter(Dictionary<string, object> evtData)
        {
            creature.headDir = memory.Get<Vector3>("targetPos", false) - transform.position;
        }

        public override void StateExit()
        {
        }

        public override void StateUpdate()
        {
        }
    }
}
