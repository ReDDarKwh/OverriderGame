using Bolt;
using UnityEngine;
using System.Linq;
using Scripts.Actions;
using System.Collections.Generic;
using System.Collections;

namespace Scripts.States
{
    public class UnsureState : AbstractState
    {
        public ExternalLogicAction chasingAction;
        public Creature creature;
        private GameObject target;
        public SoundPreset unsureSound;
        public SoundPreset scanningSound;
        public float defaultUnsureTime;
        private AudioSource scanningAudio;
        private Coroutine coroutine;

        public void MakeSound()
        {
            if (unsureSound)
            {
                unsureSound.Play(transform.position);
                scanningAudio = scanningSound.Play(transform.position);
            }
        }

        public override void StateUpdate()
        {
            if (target)
            {
                creature.headDir = target.transform.position - transform.position;
            }
        }

        public override void StateExit()
        {
            if (scanningAudio)
            {
                SoundManager.Instance.Stop(scanningAudio);
            }

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        public override void StateEnter()
        {

            MakeSound();

            var lastTarget = memory.Get<GameObject>("target");

            if (false)
            {
                // TODO: Make random selection work for cleaning bot
                var i = 0;
                do
                {
                    target = chasingAction.dataInputs["Targets"].ElementAt(Random.Range(0, chasingAction.dataInputs["Targets"].Count));
                    i++;
                }
                while (lastTarget == target && i < 10);
            }
            else
            {
                target = chasingAction.dataInputs["Targets"].FirstOrDefault();
            }

            if (lastTarget == target)
            {
                // for cleaning bot
                CustomEvent.Trigger(gameObject, "NoNewTarget");
            }

            memory.Set("target", target);
             
            var memuUnsureTime = memory.Get<float>("unsureTime");

            coroutine = StartCoroutine(
                WaitAndTrigger(memuUnsureTime == 0? defaultUnsureTime : memuUnsureTime, root, "isAlert")
            );
        }
    
    }
}