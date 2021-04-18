using System.Collections.Generic;
using UnityEngine;

namespace Scripts.States
{
    public abstract class AbstractState : MonoBehaviour
    {
        internal bool isRunning;

        public abstract void StateEnter(Dictionary<string, object> evtData);
        public abstract void StateUpdate();
        public abstract void StateExit();

        void Update()
        {
            if (isRunning)
            {
                StateUpdate();
            }
        }
    }
}



