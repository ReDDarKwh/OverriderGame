using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Scripts.States
{
    public class GotoState : AbstractState
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

        public override void StateEnter()
        {
            var gotoSettingsName = memory.Get<string>("gotoSettingsName", false);
            var targetTransform = memory.Get<Transform>("targetTransform", false);
            var targetPos = memory.Get<Vector3>("targetPos", false);

            this.lookAtTarget = memory.Get<bool>("lookAtTarget", false);
            this.atPositionEventName = memory.Get<string>("positionEventName", false);

            if (moveSettingsRepo == null)
            {
                moveSettingsRepo = moveSettings.ToDictionary(x => x.name);
            }

            creature.nav.SetSpeed(moveSettingsRepo[gotoSettingsName].moveSpeed);
            targetRange = moveSettingsRepo[gotoSettingsName].targetRange;

            if (targetTransform != null)
            {
                this.targetTransform = targetTransform;
                creature.nav.SetTarget(this.targetTransform, targetRange);
                isMovingObject = true;
            }
            else
            {
                this.targetPos = targetPos;
                creature.nav.SetTarget(this.targetPos, targetRange);
                isMovingObject = false;
            }
        }

        public void IsAtPosition(){
            root.TriggerEvent(atPositionEventName);
        }

        public override void StateUpdate()
        {
            var v = lookAtTarget ? (isMovingObject ? targetTransform.position : targetPos) - transform.position : creature.nav.GetDir();
            creature.headDir = v;
        }

        public override void StateExit()
        {
            targetTransform = null;
            creature.nav.Stop();
        }
    }
}