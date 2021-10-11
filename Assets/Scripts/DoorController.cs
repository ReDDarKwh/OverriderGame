using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Actions;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class DoorController : MonoBehaviour
{
    public OpeningAction openingAction;
    public NavMeshObstacle obstacle;
    public UniqueId uniqueId;
    public float enemyNoticeDoorOpenDistance;

    public float lockTime;
    private float lockStartTime;
    public LayerMask doorLayerMask;
    public Collider2D[] doorColliders;
    private List<Pathfinding.GraphNode> nodesUnderDoor = new List<Pathfinding.GraphNode>();
    private Dictionary<Creature, float> lockedCreatures = new Dictionary<Creature, float>();

    void Start()
    {
        var gg = AstarPath.active.data.gridGraph;

        gg.GetNodes(node => {

            var col = Physics2D.OverlapPoint((Vector3)node.position, doorLayerMask)?.gameObject.GetComponent<Door>()?.doorController;

            if(col != null && col == this){
                nodesUnderDoor.Add(node);
            };
        });
    }

    internal void SetIsLocked(Creature creature)
    {
        if(creature == null){
            return;
        }

        if(lockedCreatures.ContainsKey(creature)){
            lockedCreatures[creature] = Time.time;
        } else {
            lockedCreatures.Add(creature, Time.time);
            creature?.nav.LockNodes(uniqueId.uniqueId, nodesUnderDoor);        
        }
    }

    // Update is called once per frame
    void Update()
    {
        var removeQueue = new List<Creature>();
        
        foreach(var keyval in lockedCreatures){

            if (Time.time - keyval.Value > lockTime || 
                (
                    openingAction.outputGate.currentValue &&  
                    keyval.Key.spottingAction.TargetInView(this.gameObject) != null
                )
            )
            {
                removeQueue.Add(keyval.Key);
                keyval.Key.nav.UnblockNodes(uniqueId.uniqueId);
            }
        }

        foreach(var r in removeQueue){
            lockedCreatures.Remove(r);
        }
    }
}
