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

public class PathFindingParams : MonoBehaviour
{
    public UnityEvent targetUnreachableEvent;
    public UnityEvent targetReachedEvent;
    public float refreshDis;
    public LayerMask doorLayer;
}
