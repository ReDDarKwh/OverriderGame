using System.Collections.Generic;
using UnityEngine;

namespace Scripts.States
{
    public class AlertingState : AbstractState
    {
        public NoiseEmitter alertVoice;
        
        public override void StateEnter()
        {
            alertVoice.interactableObject = memory.Get<GameObject>("target", false);
            alertVoice.EmitNoise();            
        }

        public override void StateExit()
        {
        }

        public override void StateUpdate()
        {
        }
    }
}



