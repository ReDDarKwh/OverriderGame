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

        [System.NonSerialized]
        public float enterTime;
        private bool isRunning;

        public abstract void StateEnter();
        public abstract void StateUpdate();
        public abstract void StateExit();

        public void PreEnterState(){
            enabled = true;
            enterTime = Time.time;
            StateEnter();
            if(root == null){
                Start();
            }
            root.TriggerEvent("enter");
        }
        
        void Update(){
            StateUpdate();
        }

        public void PreExitState(){
            enabled = false;
            StateExit();
        }

        public float getStateRunTime()
        {
            return Time.time - enterTime;
        }

        void Start()
        {
            memory = GetComponent<StateMachineMemory>();
            root = GetComponent<HSM>();
            enabled = false;
        }

        public IEnumerator WaitAndTrigger(float waitTime, string eventName)
        {
            yield return new WaitForSeconds(waitTime);
            root.TriggerEvent(eventName);
        }
    }
}



