using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

namespace Scripts.States
{
    public class CuriousState : AbstractState
    {
        private GameObject target;

        public override void StateEnter(Dictionary<string, object> evtData)
        {
            target = Variables.Object(gameObject).Get<GameObject>("target");

            if (target)
            {
                Variables.Object(gameObject).Set("lastTargetPos", target.transform.position);
            }

            memory.Set("targetPos", target.transform.position);
        }

        public override void StateExit()
        {
        }

        public override void StateUpdate()
        {
        }
    }
}