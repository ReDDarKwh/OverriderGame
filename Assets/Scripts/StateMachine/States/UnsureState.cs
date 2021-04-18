using Bolt;
using UnityEngine;
using System.Linq;
using Scripts.Actions;
using System.Collections.Generic;

namespace Scripts.States
{
    public class UnsureState : AbstractState
    {
        public ExternalLogicAction chasingAction;
        public Creature creature;
        private GameObject target;
        public SoundPreset unsureSound;
        public SoundPreset scanningSound;
        private AudioSource scanningAudio;

        public void MakeSound()
        {
            if (unsureSound)
            {
                unsureSound.Play(transform.position);
                scanningAudio = scanningSound.Play(transform.position);
            }
        }

        public void StateEnter(bool randomTargetSelection = false)
        {
            var lastTarget = Variables.Object(gameObject).Get<GameObject>("target");

            if (randomTargetSelection)
            {
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

            Variables.Object(gameObject).Set("target", target);
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
        }

        public override void StateEnter(Dictionary<string, object> evtData)
        {
        }
    }
}