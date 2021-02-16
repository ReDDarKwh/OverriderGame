using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;
using System.Linq;

public class Chasing : MonoBehaviour
{
    public Creature creature;
    public ExternalLogicAction chasingAction;
    private GameObject target;
    public float attackRange;
    public float chasingSpeed;
    public SoundPreset chasingSound;

    public void MakeChasingSound()
    {
        if (chasingSound)
        {
            SoundManager.Instance.Make(chasingSound, transform.position);
        }
    }

    public void StateEnter()
    {
        target = Variables.Object(gameObject).Get<GameObject>("target");
        creature.nav.SetTarget(target.transform);
        creature.nav.SetSpeed(chasingSpeed);

        if (chasingAction != null)
        {
            chasingAction.actionGate.SetValue(true);
        }
    }

    public void StateUpdate()
    {
        Variables.Object(gameObject).Set("lastTargetPos", target.transform.position);

        if ((target.transform.position - transform.position).magnitude < attackRange)
        {
            CustomEvent.Trigger(gameObject, "targetInAttackRange");
        }
        creature.headDir = target.transform.position - transform.position;
    }

    public void StateExit()
    {
        creature.nav.Stop();
    }
}
