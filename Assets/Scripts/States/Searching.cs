using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;
using System.Linq;

public class Searching : MonoBehaviour
{
    public ExternalLogicAction chasingAction;
    public Creature creature;
    public float targetRange;
    private Vector3 lastPos;
    private Transform targetTranform;
    private bool isMovingObject;

    public void StateEnter(bool isMovingObject = false)
    {

        creature.nav.Stop();

        this.isMovingObject = isMovingObject;

        if (isMovingObject)
        {
            targetTranform = Variables.Object(gameObject).Get<GameObject>("target").transform;
            creature.nav.SetTarget(targetTranform);
        }
        else
        {
            lastPos = Variables.Object(gameObject).Get<Vector3>("lastTargetPos");
            creature.nav.SetTarget(lastPos);
        }

        chasingAction.actionGate.SetValue(false);
    }

    public void StateUpdate()
    {
        creature.headDir = creature.nav.GetDir();

        if (creature.nav.IsTargetUnreachable())
        {
            CustomEvent.Trigger(this.gameObject, "SearchDone");
        }

        if ((transform.position - (isMovingObject ? targetTranform.position : lastPos)).magnitude < targetRange)
        {
            CustomEvent.Trigger(gameObject, "atLastSeenPos");
            creature.nav.Stop();
        }
    }

    public void StateExit()
    {
        creature.nav.Stop();
    }

}
