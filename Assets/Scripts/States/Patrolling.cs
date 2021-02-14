using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class Patrolling : MonoBehaviour
{
    public Creature creature;
    public WaypointFollower waypointFollower;
    public float patrollingSpeed;

    public void StateEnter()
    {
        waypointFollower.enabled = true;
        creature.nav.SetSpeed(patrollingSpeed);
    }

    public void StateUpdate()
    {
        creature.headDir = creature.nav.GetDir();

        if (waypointFollower.pathFindingNav.IsTargetUnreachable())
        {
            CustomEvent.Trigger(this.gameObject, "Stuck");
            waypointFollower.requestPath = true;
        }
    }

    public void StateExit()
    {
        waypointFollower.enabled = false;
    }
}
