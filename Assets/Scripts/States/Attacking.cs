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
    public float bulletSpeed;

    private GameObject target;
    private Creature targetCreature;
    private PlayerController targetPlayerController;
    private Rigidbody2D targetRb;
    public float attackRangePlusBuffer;

    public void StateEnter()
    {
        target = Variables.Object(gameObject).Get<GameObject>("target");
        targetCreature = target.GetComponent<Creature>();
        targetPlayerController = target.GetComponent<PlayerController>();

        shootingAction.actionGate.SetValue(true);
    }

    public void StateUpdate()
    {
        Variables.Object(gameObject).Set("lastTargetPos", target.transform.position);

        if ((target.transform.position - transform.position).magnitude > attackRangePlusBuffer)
        {
            CustomEvent.Trigger(gameObject, "targetOutOfAttackRange");
        }
        creature.headDir = (target.transform.position - transform.position);

        var bulletTravelTime = (target.transform.position - transform.position).magnitude / bulletSpeed;
        var positionToAimAt = target.transform.position + (GetTargetDirection().normalized * bulletTravelTime) - transform.position;

        gun.rotation = Quaternion.LookRotation(Vector3.forward, positionToAimAt) * Quaternion.Euler(0, 0, 90);
    }

    Vector3 GetTargetDirection()
    {
        if (targetPlayerController != null)
        {
            return targetPlayerController.vel;
        }
        else if (targetCreature != null)
        {
            return targetCreature.headDir;
        }
        return Vector3.zero;
    }

    public void StateExit()
    {
        shootingAction.actionGate.SetValue(false);
    }

}
