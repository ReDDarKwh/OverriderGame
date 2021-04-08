using Bolt;
using Scripts.Actions;
using UnityEngine;

namespace Scripts.States
{
    public class AttackingState : AbstractState
    {
        public Creature creature;
        public Action shootingAction;
        public Transform[] guns;
        public float bulletSpeed;

        private GameObject target;
        private Creature targetCreature;
        private PlayerController targetPlayerController;
        private Rigidbody2D targetRb;
        public float attackRangePlusBuffer;

        public override void StateEnter()
        {
            target = Variables.Object(gameObject).Get<GameObject>("target");
            targetCreature = target.GetComponent<Creature>();
            targetPlayerController = target.GetComponent<PlayerController>();

            if (target.layer == LayerMask.NameToLayer("Guard"))
            {
                creature.SetHacked(true);
            }

            shootingAction.actionGate.SetValue(true);
        }

        public override void StateUpdate()
        {
            if (!target)
            {
                return;
            }

            if ((target.transform.position - transform.position).magnitude > attackRangePlusBuffer)
            {
                CustomEvent.Trigger(gameObject, "targetOutOfAttackRange");
            }
            creature.headDir = target.transform.position - transform.position;

            foreach (var gun in guns)
            {
                var bulletTravelTime = (target.transform.position - transform.position).magnitude / bulletSpeed;
                var positionToAimAt = target.transform.position + GetTargetDirection().normalized * bulletTravelTime - gun.position;

                gun.rotation = Quaternion.LookRotation(Vector3.forward, positionToAimAt) * Quaternion.Euler(0, 0, 90);
            }
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

        public override void StateExit()
        {
            shootingAction.actionGate.SetValue(false);
            creature.SetHacked(false);
        }

    }
}