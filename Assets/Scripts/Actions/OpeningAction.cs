using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Lowscope.Saving;
using System;

namespace Scripts.Actions
{
    public class OpeningAction : Action
    {
        public DoorSettings[] doors;
        public float movingSpeed;
        private Vector3[] doorStartPos;
        private float pos;

        [Serializable]
        public class OpeningSaveData : Action.SaveData
        {
            public float pos;
        }

        public override void OnLoad(string data)
        {
            base.OnLoad(data);
            var pos = JsonUtility.FromJson<OpeningSaveData>(data).pos;
            this.pos = pos;
        }

        public override string OnSave()
        {
            return JsonUtility.ToJson(
                new OpeningSaveData { 
                    pos = pos, 
                    inputGateValue = inputGate?.currentValue ?? false 
                });
        }

        internal bool IsDoorClosing()
        {
            return !outputGate.currentValue && pos > 0;
        }

        internal override void OnStart()
        {
            doorStartPos = doors.Select(x => x.door.position).ToArray();
        }

        // Update is called once per frame
        void Update()
        {
            pos = Mathf.Clamp(pos + Time.deltaTime * movingSpeed * (outputGate.currentValue ? 1 : -1), 0, 1);
            for (var i = 0; i < doors.Count(); i++)
            {
                var doorData = doors[i];

                doorData.door.transform.position = Vector3.Lerp(doorStartPos[i],
                    doorStartPos[i] + doorData.openPositionOffset,
                    Mathf.SmoothStep(0.0f, 1.0f, pos)
                );
                doorData.doorCollider.enabled = !outputGate.currentValue;
            }
        }
    }
}