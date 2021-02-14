using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathFindingNav : Navigation
{
    public NavMeshAgent navMeshAgent;
    public float refreshDis;
    public LayerMask doorLayer;
    private Transform movingTarget;
    private Vector3 lastDesiredVelocity;

    public override Vector3 GetDir()
    {
        return (!navMeshAgent.hasPath || navMeshAgent.pathPending) ? lastDesiredVelocity : navMeshAgent.desiredVelocity;
    }

    public override void SetSpeed(float speed)
    {
        navMeshAgent.speed = speed;
    }

    public override void SetTarget(Transform target)
    {
        movingTarget = target;
        SetTarget(movingTarget.position);
    }

    public override void SetTarget(Vector3 target)
    {
        navMeshAgent.isStopped = stopped = false;
        navMeshAgent.SetDestination(target);
    }

    public override void Stop()
    {
        navMeshAgent.isStopped = stopped = true;
        movingTarget = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent.autoBraking = false;
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        navMeshAgent.autoRepath = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (movingTarget != null && navMeshAgent.hasPath)
        {
            if ((navMeshAgent.destination - movingTarget.position).magnitude > refreshDis)
            {
                SetTarget(movingTarget);
            }
        }

        if (navMeshAgent.hasPath)
        {
            lastDesiredVelocity = navMeshAgent.desiredVelocity;
        }
    }

    public bool IsGoingThroughDoor(DoorController doorController)
    {
        if (this.navMeshAgent.hasPath)
        {
            var lastCorner = transform.position;
            foreach (var c in this.navMeshAgent.path.corners)
            {
                var hit = Physics2D.Linecast(lastCorner, c, doorLayer);
                if (hit.collider != null)
                {
                    var door = hit.collider.GetComponent<Door>();
                    if (door != null)
                    {
                        return door.doorController == doorController;
                    }
                }

                lastCorner = c;
            }
        }
        return false;
    }

    void OnEnable()
    {
        navMeshAgent.enabled = true;
    }

    void OnDisable()
    {
        navMeshAgent.enabled = false;
    }

    public override bool IsMoving()
    {
        return (navMeshAgent.hasPath && !navMeshAgent.pathPending && !stopped);
    }

    public override bool IsTargetUnreachable()
    {
        return navMeshAgent.hasPath && navMeshAgent.path.status == NavMeshPathStatus.PathPartial;
    }
}
