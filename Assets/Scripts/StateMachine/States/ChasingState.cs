using System.Collections.Generic;
using Scripts.Actions;
using UnityEngine;

namespace Scripts.States
{
    public class ChasingState : AbstractState
    {
        public ExternalLogicAction chasingAction;
        public SoundPreset chasingSound;
        public NoiseEmitter chasingNoise;

        public override void Init()
        {
            base.Init();
            memory.Set("chasingAction", chasingAction);
        }

        public void MakeChasingSound()
        {
            if (chasingSound)
            {
                SoundManager.Instance.Make(chasingSound, transform.position);
            }
        }

        public override void StateEnter()
        {
            if (chasingAction != null)
            {
                chasingAction.actionGate.SetValue(true);
            }

            HSM.SetUpGoto(memory, null, memory.Get<GameObject>("target", false)?.transform, "chasing", true);

            if (chasingNoise)
            {
                chasingNoise.EmitNoise();
            }

            root.TriggerEvent("done");
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