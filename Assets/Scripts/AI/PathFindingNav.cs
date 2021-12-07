using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System;
using UnityEngine.Events;
using Lowscope.Saving;

public class PathFindingNav: MonoBehaviour, ISaveable
{
    internal bool stopped;
    public AIPath ai;
    public Seeker seeker;
    public float refreshDis;
    public LayerMask doorLayer;
    public UnityEvent targetUnreachableEvent;

    private Transform movingTarget;
    private Vector3 lastDesiredVelocity;
    public HashSet<GraphNode> blockedNodes = new HashSet<GraphNode>();

    public Dictionary<string, List<GraphNode>> blockedNodesGroups = new Dictionary<string, List<GraphNode>>();

    private CoolTraversalProvider traversalProvider;
    private bool targetUnreachable;

    public class CoolTraversalProvider : ITraversalProvider {
        public Func<HashSet<GraphNode>> GetBlockedNodes { get; internal set; }

        public bool CanTraverse (Path path, GraphNode node) {

            var blockedNodes = GetBlockedNodes.Invoke();
            var canTraverse = !blockedNodes.Contains(node) && node.Walkable && (path.enabledTags >> (int)node.Tag & 0x1) != 0;
            return canTraverse;
        }

        public uint GetTraversalCost (Path path, GraphNode node) {
            return DefaultITraversalProvider.GetTraversalCost(path, node);
        }
    }

    [Serializable]
    public struct SaveData
    {
        public Vector3 lastDesiredVelocity;
    }

    public Vector3 GetDir()
    {
        if(IsMoving())
            lastDesiredVelocity = ai.desiredVelocity;
        
        return lastDesiredVelocity;
    }

    public void SetSpeed(float speed)
    {
        ai.maxSpeed = speed;
    }

    public void SetTarget(Transform target)
    {
        movingTarget = target;
        SetTarget(movingTarget.position);
    }

    public void SetTarget(Vector3 target)
    {
        ai.isStopped = stopped = false;
        ai.destination = target;
        ai.SearchPath();
    }

    public void Stop()
    {
        ai.isStopped = stopped = true;
        movingTarget = null;
    }

    public void ClearPath()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        traversalProvider = new CoolTraversalProvider(){
            GetBlockedNodes = () => {
                return blockedNodes;
                }
        };
        
        seeker.preProcessPath += (Path p) => {
            p.traversalProvider = traversalProvider;
        };

        seeker.pathCallback += (Path p) => {
            targetUnreachable = p.CompleteState == PathCompleteState.Error || p.CompleteState == PathCompleteState.Partial;

            if(targetUnreachable){
                targetUnreachableEvent.Invoke();
            }
        };
        
    }

    void OnDestroy(){
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

    public bool IsMoving()
    {
        return (ai.hasPath && !ai.pathPending && !stopped);
    }

    private void UpdateBlockedNodes(){

        blockedNodes = new HashSet<GraphNode>(
            blockedNodesGroups.Values.SelectMany(x => x).Distinct()
        );

        ai.SearchPath();
    }

    internal void LockNodes(string groupId, List<GraphNode> nodesUnderDoor)
    {
        if(!blockedNodesGroups.ContainsKey(groupId)){
            blockedNodesGroups.Add(groupId, nodesUnderDoor);
        }
        UpdateBlockedNodes();
    }

    internal void UnblockNodes(string groupId)
    {
        blockedNodesGroups.Remove(groupId);
        UpdateBlockedNodes();
    }

    public string OnSave()
    {
        return JsonUtility.ToJson(new SaveData { lastDesiredVelocity = lastDesiredVelocity });
    }

    public void OnLoad(string data)
    {
        lastDesiredVelocity = JsonUtility.FromJson<SaveData>(data).lastDesiredVelocity;
    }

    public bool OnSaveCondition()
    {
        return this != null && this.gameObject.activeSelf;
    }
}
