using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using Scripts.Hacking;
using UnityEngine;

namespace Scripts.Actions
{
    public class TargetingAction : Action
    {
        public float damping;
        public float bulletSpeed;
        internal IEnumerable<Transform> targets;
        private Quaternion initialRotation;
        private DataGate targetsDataInput;
        internal Transform target;

        internal override void OnStart()
        {
            initialRotation = transform.rotation;

            targetsDataInput = new DataGate { name = "targets" };
            dataGates.Add(targetsDataInput);

            targetsDataInput.ValueHasChanged += targetsDataInput_ValueChanged;
        }

        private void targetsDataInput_ValueChanged(object sender, System.EventArgs e)
        {
            targets = targetsDataInput.GetData<GameObject>().Select(x => x.transform);
        }

        // Update is called once per frame
        void Update()
        {
            target = (targets?.Any() ?? false)? targets?.MinBy(x => (transform.position - x.transform.position).sqrMagnitude) : null;
            Quaternion desiredRotQ = Quaternion.identity;
            if (target != null)
            {
                var bulletTravelTime = (target.transform.position - transform.position).magnitude / bulletSpeed;
                var turretToTarget = target.transform.position + GetTargetDirection(target).normalized * bulletTravelTime - transform.position;
                desiredRotQ = Quaternion.LookRotation(Vector3.forward, turretToTarget) * Quaternion.Euler(0, 0, 90);
                actionGate.SetValue(true);
            }
            else
            {
                desiredRotQ = initialRotation;
                actionGate.SetValue(false);
            }

            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotQ, Time.deltaTime * damping);
        }

        Vector3 GetTargetDirection(Transform target)
        {
            var targetCreature = target.GetComponent<Creature>();
            var targetPlayerController = target.GetComponent<PlayerController>();

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
    }
}