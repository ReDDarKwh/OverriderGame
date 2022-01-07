using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Hacking;
using UnityEngine;

namespace Scripts.Actions
{
    public class SpottingAction : Action
    {
        public float VisionRadius;
        public int VisionAngle;

        [ColorUsageAttribute(true, true)]
        public Color OffFieldOfViewColor;
        [ColorUsageAttribute(true, true)]
        public Color OnFieldOfViewColor;

         [ColorUsageAttribute(true, true)]
        public Color OffFieldOfViewLineColor;
        [ColorUsageAttribute(true, true)]
        public Color OnFieldOfViewLineColor;

        public LayerMask ViewBlockingLayers;
        public LayerMask TargetLayers;
        public FieldOfView fieldOfView;
        public DataGate intrudersDataOutput;
        public bool disableFilterInput;
        public bool disableUsersOutput;
        public string intrudersDataOutputName = "Intruders";
        public string filterDataInputName = "Filter";
        public GameObject parent;
        private DataGate filterDataInput;

        // Update is called once per frame
        void Update()
        {
            var targets = GetTargetsInViewRange()
                .Where(x => x != null)
                .Select(x => TargetInView(x.gameObject))
                .Where(x => x != null);
            intrudersDataOutput.SetData(targets == null ? null : targets);
            intrudersDataOutput.SetValue(targets != null && targets.Count() > 0);
            actionGate.SetValue(targets.Count() != 0);

            if (fieldOfView != null)
            {
                fieldOfView.viewAngle = VisionAngle;
                fieldOfView.viewRadius = VisionRadius;
                fieldOfView.color = outputGate.currentValue? OnFieldOfViewColor: OffFieldOfViewColor;
                fieldOfView.lineColor = outputGate.currentValue? OnFieldOfViewLineColor: OffFieldOfViewLineColor;
            }
        }

        public GameObject TargetInView(GameObject target)
        {
            if (target == null)
                return null;

            if (parent != null)
            {
                if (target.GetInstanceID() == parent.GetInstanceID())
                    return null;
            }

            var EnemyToTargetVec = target.transform.position - transform.position;

            // if is in view cone
            if (!(Quaternion.Angle(transform.rotation,
                Quaternion.LookRotation(Vector3.forward,
                Quaternion.Euler(0, 0, 90) * EnemyToTargetVec)
                ) < VisionAngle / 2))
                return null;

            // if not blocked by any walls
            var hit = Physics2D.Raycast(
                transform.position,
                EnemyToTargetVec,
                EnemyToTargetVec.magnitude,
                ViewBlockingLayers
            );

            return hit.collider == null ? target : null;
        }

        public IList<Collider2D> GetTargetsInViewRange()
        {
            var mask = filterDataInput.GetSingleData<int>();
            return Physics2D.OverlapCircleAll(
                transform.position,
                VisionRadius,
                mask == 0 ? (int)TargetLayers : mask
            );
        }

        private Collider2D GetClosestTargetInViewRange()
        {
            var targets = GetTargetsInViewRange();
            return targets.OrderBy(x => (transform.position - x.transform.position).magnitude)
            .FirstOrDefault();
        }

        public GameObject ClosestTargetInView()
        {
            return TargetInView(GetClosestTargetInViewRange()?.gameObject);
        }

        internal override void OnStart()
        {
            disableInput = true;

            intrudersDataOutput = new DataGate(false)
            {
                name = intrudersDataOutputName,
                dataGateType = DataGate.DataGateType.Output
            };

            if (!disableUsersOutput)
            {
                dataGates.Add(intrudersDataOutput);
            }

            filterDataInput = new DataGate(true)
            {
                name = filterDataInputName,
                dataGateType = DataGate.DataGateType.Input,
                dataType = DataGate.DataType.Filters,
                maxInputs = 1
            };

            if (!disableFilterInput)
            {
                dataGates.Add(filterDataInput);
            }

            if (fieldOfView != null)
            {
                fieldOfView.viewAngle = VisionAngle;
                fieldOfView.viewRadius = VisionRadius;
                fieldOfView.obstacleMask = ViewBlockingLayers;
            }
        }

        void OnDisable()
        {
            if (fieldOfView != null)
            {
                fieldOfView.gameObject.SetActive(false);
            }
        }

        void OnEnable()
        {
            if (fieldOfView != null)
            {
                fieldOfView.gameObject.SetActive(true);
            }
        }
    }
}