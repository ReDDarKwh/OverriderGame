using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System;
using UnityEngine.Events;
using Lowscope.Saving;
using Lowscope.Saving.Components;

public class PathFindingNav: Pathfinding.AIPath, ISaveable
{
    private UnityEvent targetUnreachableEvent;
    private UnityEvent targetReachedEvent;
    private float refreshDis;
    private LayerMask doorLayer;
    private Dictionary<string, List<GraphNode>> blockedNodesGroups = new Dictionary<string, List<GraphNode>>();
    private bool stopped;
    private Vector3 lastDesiredVelocity;
    private HashSet<GraphNode> blockedNodes = new HashSet<GraphNode>();
    private Transform movingTarget;
    private float targetRange;
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
        lastDesiredVelocity = velocity2D;
        return lastDesiredVelocity;
    }

    public void SetSpeed(float speed)
    {
        maxSpeed = speed;
    }

    public void SetTarget(Transform target, float targetRange)
    {
        movingTarget = target;
        this.targetRange = targetRange;
        SetTarget(movingTarget.position, targetRange);
    }

    public void SetTarget(Vector3 target, float targetRange)
    {
        if(IsAtPosition(target, targetRange)){
            OnTargetReached();
            return;
        };

        endReachedDistance = targetRange;
        isStopped = stopped = false;
        destination = target;
        SearchPath();
    }

    private bool IsAtPosition(Vector3 target, float targetRange)
    {
        return (transform.position - target).magnitude < targetRange;
    }

    public void Stop()
    {
        SetPath(null);
        isStopped = stopped = true;
        movingTarget = null;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        var p = GetComponent<PathFindingParams>();
        doorLayer = p.doorLayer;
        targetReachedEvent = p.targetReachedEvent;
        targetUnreachableEvent = p.targetUnreachableEvent;
        refreshDis = p.refreshDis;

        base.Start();

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

    public override void OnTargetReached(){
        Debug.LogWarning("Path Reached");
        targetReachedEvent.Invoke();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (movingTarget != null && hasPath)
        {
            if ((destination - movingTarget.position).magnitude > refreshDis)
            {
                SetTarget(movingTarget, targetRange);
            }
        }
    }

    public bool IsGoingThroughDoor(DoorController doorController)
    {
        if (hasPath)
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

    public bool IsMoving()
    {
        return (hasPath && !pathPending && !stopped);
    }

    private void UpdateBlockedNodes(){

        blockedNodes = new HashSet<GraphNode>(
            blockedNodesGroups.Values.SelectMany(x => x).Distinct()
        );

        SearchPath();
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
        ResetPathfinding();
        lastDesiredVelocity = JsonUtility.FromJson<SaveData>(data).lastDesiredVelocity;
    }

    public void ResetPathfinding(){
        enabled = false;
        enabled = true;
    }

    public bool OnSaveCondition(bool isLevelSave)
    {
        return !isLevelSave && this != null && this.gameObject.activeSelf;
    }

}
