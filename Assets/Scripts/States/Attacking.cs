using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;
using System.Linq;
using System;

public class Attacking : MonoBehaviour
{
    public Creature creature;
    public ShootingAction shootingAction;
    public Transform gun;

    private GameObject target;
    public float attackRangePlusBuffer;

    public void StateEnter()
    {
        target = Variables.Object(gameObject).Get<GameObject>("target");
        shootingAction.actionGate.SetValue(true);
    }

    public void StateUpdate()
    {
        if ((target.transform.position - transform.position).magnitude > attackRangePlusBuffer)
        {
            CustomEvent.Trigger(gameObject, "targetOutOfAttackRange");
        }
        creature.headDir = (target.transform.position - transform.position);

        gun.rotation = Quaternion.LookRotation(Vector3.forward, target.transform.position - transform.position) * Quaternion.Euler(0, 0, 90);
    }

    public void StateExit()
    {
        shootingAction.actionGate.SetValue(false);
    }

}
