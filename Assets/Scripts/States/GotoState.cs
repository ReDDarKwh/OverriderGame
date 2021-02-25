using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;
using System.Linq;

public class GotoState : MonoBehaviour
{
    public Creature creature;
    private Vector3 targetPos;
    private Transform targetTransform;
    private bool isMovingObject;
    private float targetRange;

    public void StateEnter(Transform targetTransform, Vector3 targetPos, float targetRange, float speed)
    {
        creature.nav.Stop();
        creature.nav.SetSpeed(speed);

        this.targetRange = targetRange;
        if (targetTransform != null)
        {
            this.targetTransform = targetTransform;
            creature.nav.SetTarget(this.targetTransform);
            isMovingObject = true;
        }
        else
        {
            this.targetPos = targetPos;
            creature.nav.SetTarget(this.targetPos);
            isMovingObject = false;
        }
    }

    public void StateUpdate()
    {

        creature.headDir = creature.nav.GetDir();

        if (creature.nav.IsTargetUnreachable())
        {
            CustomEvent.Trigger(this.gameObject, "isUnreachable");
        }

        if ((transform.position - (isMovingObject ? targetTransform.position : targetPos)).magnitude < targetRange)
        {
            CustomEvent.Trigger(gameObject, "isAtPosition");
        }
    }

    public void StateExit()
    {
        creature.nav.Stop();
        this.targetTransform = null;
    }

}
