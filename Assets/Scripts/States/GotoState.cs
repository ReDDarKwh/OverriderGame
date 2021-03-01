using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;
using System.Linq;

public class GotoState : MonoBehaviour
{
    public Creature creature;
    public MoveSetting[] moveSettings;
    private Vector3 targetPos;
    private Transform targetTransform;

    private bool isMovingObject;
    private float targetRange;

    private Dictionary<string, MoveSetting> moveSettingsRepo;


    public void StateEnter(Transform targetTransform, Vector3 targetPos, string settingName)
    {
        if (moveSettingsRepo == null)
        {
            moveSettingsRepo = moveSettings.ToDictionary(x => x.name);
        }

        creature.nav.Stop();
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
    }

    public void StateUpdate(string atPositionEventName = "isAtPosition")
    {

        creature.headDir = creature.nav.GetDir();

        if (creature.nav.IsTargetUnreachable())
        {
            CustomEvent.Trigger(this.gameObject, "isUnreachable");
        }

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
