using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class WaypointFollower : MonoBehaviour
{
    public PathFindingNav pathFindingNav;
    public Transform[] waypoints;
    public float speed;
    public float targetRadius;

    private Vector3 start;
    private Vector3 end;
    internal int currentPoint;
    private int? savedPoint;
    internal bool requestPath;

    // Start is called before the first frame update
    void Start()
    {
        currentPoint = -1;
        requestPath = true;
        OnEnable();
    }

    void OnEnable()
    {
        if (currentPoint != -1)
        {
            pathFindingNav.SetTarget(waypoints[currentPoint]);
        }
    }

    void OnDisable()
    {
        pathFindingNav.navMeshAgent.ResetPath();
        pathFindingNav.Stop();
    }

    internal int GetNextPoint()
    {
        return (currentPoint + 1) % waypoints.Count();
    }

    // Update is called once per frame
    void Update()
    {
        if ((!pathFindingNav.navMeshAgent.hasPath && !pathFindingNav.navMeshAgent.pathPending) || requestPath)
        {
            requestPath = false;
            currentPoint = GetNextPoint();
            pathFindingNav.SetTarget(waypoints[currentPoint]);
        }

        if ((transform.position - waypoints[currentPoint].position).magnitude < targetRadius)
        {
            pathFindingNav.navMeshAgent.ResetPath();
        }
    }

}
