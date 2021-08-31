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
            isRunning = true;
            enterTime = Time.time;
            if(root == null){
                Start();
            }
            StateEnter();
            root.TriggerEvent("enter");
        }
        
        void Update(){
            if(isRunning){
                StateUpdate();
            }
        }

        public void PreExitState(){
            isRunning = false;
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
        }

        public IEnumerator WaitAndTrigger(float waitTime, string eventName)
        {
            yield return new WaitForSeconds(waitTime);
            root.TriggerEvent(eventName);
        }
    }
}



