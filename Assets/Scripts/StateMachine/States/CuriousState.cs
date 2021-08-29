using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

namespace Scripts.States
{
    public class CuriousState : AbstractState
    {
        private GameObject target;

        public override void StateEnter()
        {
            target = memory.Get<GameObject>("target");

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