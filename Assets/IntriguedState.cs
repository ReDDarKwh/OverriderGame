using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntriguedState : MonoBehaviour
{
    public Creature creature;
    public void StateEnter(Vector3 targetPos)
    {
        creature.headDir = targetPos - transform.position;
    }
}
