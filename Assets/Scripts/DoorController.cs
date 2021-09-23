using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Actions;
using UnityEngine;
using UnityEngine.AI;

public class DoorController : MonoBehaviour
{
    public OpeningAction openingAction;
    public NavMeshObstacle obstacle;
    public float lockTime;
    private float lockStartTime;
    public LayerMask doorLayerMask;

    void Start()
    {

        var gg = AstarPath.active.data.gridGraph;

        gg.GetNodes(node => {
            // TODO : Find nodes

            Physics2D.OverlapPoint((Vector3)node.position, doorLayerMask);
        });
    }

    internal void SetIsLocked()
    {
        obstacle.enabled = true;
        lockStartTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Time.time - lockStartTime > lockTime && obstacle.enabled) || openingAction.outputGate.currentValue)
        {
            obstacle.enabled = false;
        }

        // if (openingAction.outputGate != null)
        // {
        //     obstacle.enabled = !openingAction.outputGate.currentValue;
        // }
    }
}
