using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lowscope.Saving;
using Lowscope.Saving.Components;
using UnityEngine;

namespace Scripts.States
{

   
    public class PatrollingState : AbstractState, ISaveable
    {
        public Creature creature;
        public Transform[] waypoints;
        public Transform stationnaryTransform;
        public GameObject waypointPrefab;
        internal int currentPoint = -1;

        [Serializable]
        public struct SaveData
        {
            public int currentPoint;
        }

        public void Init()
        {
            if (stationnaryTransform)
            {
                var inst = Instantiate(waypointPrefab, stationnaryTransform.position, stationnaryTransform.rotation)
                    .GetComponent<Transform>();

                waypoints = new Transform[] { inst };
            }
        }

        internal Vector3 GetNextPoint()
        {
            return waypoints[currentPoint = (currentPoint + 1) % waypoints.Count()].position;
        }

        public void AtStationnaryOutpost()
        {
            if(waypoints.Count() == 0){
                Init();
            }
            creature.headDir = waypoints[0].right;
        }

        public override void StateUpdate()
        {
        }

        public override void StateExit()
        {
        }

        public string OnSave()
        {
            return JsonUtility.ToJson(new SaveData { currentPoint = currentPoint });
        }

        public void OnLoad(string data)
        {
            currentPoint = JsonUtility.FromJson<SaveData>(data).currentPoint;
        }

        public bool OnSaveCondition(bool v)
        {
            return !v;
        }

        public override void StateEnter()
        {
            Vector3 nextPoint = Vector3.zero;

            if (stationnaryTransform)
            {
                if (waypoints.Count() == 0)
                {
                    Init();
                }
                nextPoint = GetNextPoint();

            }
            else
            {
                if (waypoints.Count() > 0)
                {
                    nextPoint = GetNextPoint();
                }
            }

            HSM.SetUpGoto(memory == null? GetComponent<StateMachineMemory>() : memory, nextPoint, null, "patrolling", false);
            (root == null? GetComponent<HSM>(): root).TriggerEvent("hasPatrolPoint");
        }
    }
}