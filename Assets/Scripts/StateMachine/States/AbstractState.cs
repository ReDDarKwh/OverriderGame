using UnityEngine;

namespace Scripts.States
{
    public abstract class AbstractState : MonoBehaviour
    {
        public bool isRunning;

        public abstract void StateEnter();
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



