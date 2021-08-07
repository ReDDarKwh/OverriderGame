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

        protected T GetVar<T>(string variableName, Dictionary<string, object> data)
        {
            object result;
            var found = data.TryGetValue(variableName, out result);
            return found ? (T)result : default(T);
        }

        protected HSM GetRoot(Dictionary<string, object> data)
        {
            return GetVar<HSM>("root", data);
        }

        protected void SetUpGoto(Vector3? targetPos, Transform targetTransform, string gotoSettingsName, bool lookAtTarget)
        {
            if (targetPos != null)
            {
                memory.Set("targetPos", targetPos.Value);
            }
            memory.Set("targetTransform", targetTransform);
            memory.Set("gotoSettingsName", gotoSettingsName);
            memory.Set("lookAtTarget", lookAtTarget);
        }

        public IEnumerator WaitAndTrigger(float waitTime, HSM hsm)
        {
            yield return new WaitForSeconds(waitTime);
            hsm.TriggerEvent("searchDone");
        }


    }
}



