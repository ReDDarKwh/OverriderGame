using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.States
{
    public abstract class AbstractState : MonoBehaviour
    {

        [System.NonSerialized]
        public StateMachineMemory memory;

        [System.NonSerialized]
        public HSM root;


        internal bool isRunning;

        [System.NonSerialized]
        public float enterTime;

        public abstract void StateEnter(Dictionary<string, object> evtData);
        public abstract void StateUpdate();
        public abstract void StateExit();
        public virtual void Init() { }

        public float getStateRunTime()
        {
            return Time.time - enterTime;
        }

        void Start()
        {
            memory = GetComponent<StateMachineMemory>();
            root = GetComponent<HSM>();

            Init();
        }

        void Update()
        {
            if (isRunning)
            {
                StateUpdate();
            }
        }

        public IEnumerator WaitAndTrigger(float waitTime, HSM hsm, string eventName)
        {
            yield return new WaitForSeconds(waitTime);
            hsm.TriggerEvent(eventName);
        }


    }
}



