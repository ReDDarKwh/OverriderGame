using System.Collections.Generic;
using Scripts.Actions;
using UnityEngine;

namespace Scripts.States
{
    public class ChasingState : AbstractState
    {
        public Creature creature;
        public ExternalLogicAction chasingAction;
        public float chasingSpeed;
        public SoundPreset chasingSound;
        public NoiseEmitter chasingNoise;

        public void MakeChasingSound()
        {
            if (chasingSound)
            {
                SoundManager.Instance.Make(chasingSound, transform.position);
            }
        }

        public override void StateEnter(Dictionary<string, object> evtData)
        {
            if (chasingAction != null)
            {
                chasingAction.actionGate.SetValue(true);
            }

            if (chasingNoise)
            {
                chasingNoise.EmitNoise();
            }
        }

        public override void StateUpdate()
        {
        }

        public override void StateExit()
        {
            if (chasingAction != null)
            {
                chasingAction.actionGate.SetValue(false);
            }
        }
    }
}