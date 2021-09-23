using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.AI;

public class PathFindingNav : Navigation
{
    public IAstarAI ai;
    public Seeker seeker;
    public float refreshDis;
    public LayerMask doorLayer;
    private Transform movingTarget;
    private Vector3 lastDesiredVelocity;
    public HashSet<GraphNode> blockedNodes = new HashSet<GraphNode>();

    private CoolTraversalProvider traversalProvider;

    public class CoolTraversalProvider : ITraversalProvider {

        public HashSet<GraphNode> blockedNodes;

        public bool CanTraverse (Path path, GraphNode node) {
            return !blockedNodes.Contains(node) && node.Walkable && (path.enabledTags >> (int)node.Tag & 0x1) != 0;
        }

        public uint GetTraversalCost (Path path, GraphNode node) {
            return DefaultITraversalProvider.GetTraversalCost(path, node);
        }
    }

    public override Vector3 GetDir()
    {
        return ai.desiredVelocity;
    }

    public override void SetSpeed(float speed)
    {
        ai.maxSpeed = speed;
    }

    public override void SetTarget(Transform target)
    {
        movingTarget = target;
        SetTarget(movingTarget.position);
    }

    public override void SetTarget(Vector3 target)
    {
        ai.isStopped = stopped = false;
        ai.destination = target;
    }

    public override void Stop()
    {
        ai.isStopped = stopped = true;
        movingTarget = null;
    }

    public override void ClearPath()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        ai = GetComponent<IAstarAI>();
        traversalProvider = new CoolTraversalProvider(){
            blockedNodes = blockedNodes
        };
        
        seeker.preProcessPath = (Path p) => {
            p.traversalProvider = traversalProvider;
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (movingTarget != null && ai.hasPath)
        {
            if ((ai.destination - movingTarget.position).magnitude > refreshDis)
            {
                SetTarget(movingTarget);
            }
        }

        // if (navMeshAgent.hasPath)
        // {
        //     if (navMeshAgent.path == NavMeshPathStatus.PathPartial)
        //     {
        //         navMeshAgent.velocity = Vector3.zero;
        //     }
        //     else
        //     {
        //         lastDesiredVelocity = navMeshAgent.desiredVelocity;
        //     }
        // }
    }

    public bool IsGoingThroughDoor(DoorController doorController)
    {
        if (this.ai.hasPath)
        {
            var lastCorner = transform.position;
            foreach (var c in this.seeker.GetCurrentPath().vectorPath)
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
        ai.canMove = true;
    }

    void OnDisable()
    {
        ai.canMove = false;
    }

    public override bool IsMoving()
    {
        return (ai.hasPath && !ai.pathPending && !stopped);
    }

    public override bool IsTargetUnreachable()
    {
        return false;
    }

}
