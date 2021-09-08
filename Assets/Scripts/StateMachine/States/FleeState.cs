using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Scripts.States
{
    public class FleeState : AbstractState
    {
        public Transform[] hiddingSpots;
        private Vector3? lastHiddingSpot;

        private Vector3 GetHiddingSpot(Transform target)
        {
            var maxDis = 0f;
            Vector3 farthest = Vector3.zero;
            foreach (var point in hiddingSpots.Where(x => lastHiddingSpot == null || x.position != lastHiddingSpot.Value))
            {
                var dis = (point.position - target.position).magnitude;
                if (dis > maxDis)
                {
                    maxDis = dis;
                    farthest = point.position;
                }
            }

            lastHiddingSpot = farthest;
            return farthest;
        }

        public override void StateEnter()
        {
            root.TriggerEvent("flee", 
                new EventData{{"hiddingPosition", GetHiddingSpot(memory.Get<GameObject>("target", false).transform)}
            });
        }

        public override void StateUpdate()
        {
        }

        public override void StateExit()
        {
        }
    }
}
