using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class Patrolling : MonoBehaviour
{
    public Creature creature;
    public WaypointFollower waypointFollower;
    public Vector3 stationnaryDirection;
    public bool stationnary;
    public float patrollingSpeed;

    public void StateEnter()
    {
        if (waypointFollower)
            waypointFollower.enabled = true;
        creature.nav.SetSpeed(patrollingSpeed);
    }

    public void StateUpdate()
    {
        if (!waypointFollower)
            return;

        creature.headDir = waypointFollower.pathFindingNav.IsMoving() ?
            creature.nav.GetDir() : stationnary ?
                stationnaryDirection : creature.nav.GetDir();

        if (waypointFollower.pathFindingNav.IsTargetUnreachable())
        {
            CustomEvent.Trigger(this.gameObject, "Stuck");
            waypointFollower.requestPath = true;
        }
    }

    public void StateExit()
    {
        if (waypointFollower)
            waypointFollower.enabled = false;
    }
}
