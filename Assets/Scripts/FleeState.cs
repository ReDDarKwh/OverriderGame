using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using System.Linq;

public class FleeState : MonoBehaviour
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

    public Vector3 StateEnter(Transform target)
    {
        return GetHiddingSpot(target);
    }
}
