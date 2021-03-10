using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;
using System.Linq;

public class GotoState : MonoBehaviour
{
    public Creature creature;
    public MoveSetting[] moveSettings;
    private bool lookAtTarget;
    private Vector3 targetPos;
    private Transform targetTransform;

    private bool isMovingObject;
    private float targetRange;

    private Dictionary<string, MoveSetting> moveSettingsRepo;
    private string atPositionEventName;

    public void StateEnter(Transform targetTransform, Vector3 targetPos, string settingName, bool lookAtTarget, string atPositionEventName = "isAtPosition")
    {

        if (moveSettingsRepo == null)
        {
            moveSettingsRepo = moveSettings.ToDictionary(x => x.name);
        }

        this.lookAtTarget = lookAtTarget;
        this.atPositionEventName = atPositionEventName;

        creature.nav.SetSpeed(moveSettingsRepo[settingName].moveSpeed);

        this.targetRange = moveSettingsRepo[settingName].targetRange;
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

        CheckIsAtPosition(atPositionEventName);
    }

    public void StateUpdate()
    {
        CheckIsAtPosition(atPositionEventName);

        if (creature.nav.IsTargetUnreachable())
        {
            CustomEvent.Trigger(this.gameObject, "isUnreachable");
        }
        else
        {
            var v = lookAtTarget ? ((isMovingObject ? targetTransform.position : targetPos) - transform.position) : creature.nav.GetDir();
            creature.headDir = v;
        }
    }

    private void CheckIsAtPosition(string atPositionEventName)
    {
        if ((transform.position - (isMovingObject ? targetTransform.position : targetPos)).magnitude < targetRange)
        {
            CustomEvent.Trigger(gameObject, atPositionEventName);
        }
    }

    public void StateExit()
    {
        creature.nav.Stop();
        this.targetTransform = null;
    }

}
