using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.States
{
    public class LookAroundState : AbstractState
    {
        public Creature creature;
        public float speed;
        public float relativeMinAngle;
        public float relativeMaxAngle;
        public float unsureTime;
        private Quaternion startRotation;
        private Coroutine waitCoroutine;
        private Quaternion minRotation;
        private Quaternion maxRotation;
        private Quaternion targetRotation;
        private float time;

        public override void StateEnter()
        {
            startRotation = Quaternion.LookRotation(Vector3.forward, creature.headDir) * Quaternion.Euler(0, 0, 90);
            waitCoroutine = StartCoroutine(WaitAndTrigger(unsureTime, root, "searchDone"));
        }

        public override void StateExit()
        {
            StopCoroutine(waitCoroutine);
        }

        public override void StateUpdate()
        {
            minRotation = startRotation * Quaternion.Euler(0, 0, -relativeMinAngle);
            maxRotation = startRotation * Quaternion.Euler(0, 0, relativeMaxAngle);

            time += Time.deltaTime;
            creature.headDir = Quaternion.Lerp(minRotation, maxRotation, (Mathf.Sin(time * speed) + 1) / 2) * Vector3.right;
        }
    }
}