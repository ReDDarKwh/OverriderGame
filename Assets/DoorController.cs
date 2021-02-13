using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoorController : MonoBehaviour
{
    public OpeningAction openingAction;
    public NavMeshObstacle obstacle;
    public float lockTime;
    private float lockStartTime;

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
    }
}
