using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bolt;
using UnityEngine;

public class PatrollingState : MonoBehaviour
{
    public Creature creature;
    public Transform[] waypoints;
    internal int currentPoint;

    void Start()
    {
        currentPoint = -1;
    }

    internal Vector3 GetNextPoint()
    {
        return waypoints[currentPoint = (currentPoint + 1) % waypoints.Count()].position;
    }

    public void StateEnter()
    {
        if (waypoints.Count() > 0)
        {
            Variables.Object(gameObject).Set("patrolPoint", GetNextPoint());
            CustomEvent.Trigger(this.gameObject, "asPatrolPoint");
        }
    }

    public void StateUpdate()
    {
    }

    public void StateExit()
    {
    }
}
