using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.States
{
    public class CuriousState : AbstractState
    {
        public float curiousTime;
        private GameObject target;

        public override void StateEnter()
        {
            target = memory.Get<GameObject>("target");

            memory.Set("targetPos", target.transform.position, MemoryType.Value);
        }

        public override void StateExit()
        {
        }

        public override void StateUpdate()
        {
        }
    }
}