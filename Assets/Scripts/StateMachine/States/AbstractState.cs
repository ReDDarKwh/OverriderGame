using System;
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


        internal bool isActive;

        [System.NonSerialized]
        public float enterTime;
        private bool isRunning;

        public abstract void StateEnter();
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
            if (isActive)
            {
                if(!isRunning){
                    StateEnter();
                    PostStateEnter();
                    isRunning = true;
                } else {
                    StateUpdate();  
                }
            } else if(isRunning){
                StateExit();
                isRunning = false;
            }
        }

        private void PostStateEnter()
        {
            root.TriggerEvent("enter");
        }

        public IEnumerator WaitAndTrigger(float waitTime, HSM hsm, string eventName)
        {
            yield return new WaitForSeconds(waitTime);
            hsm.TriggerEvent(eventName);
        }

    }
}



