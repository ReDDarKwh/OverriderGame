using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Hacking;
using UnityEngine;
using System.Linq;

namespace Scripts.Actions
{
    public class AlarmAction : Action
    {
        public ExternalLogicAction activate;
        public ExternalLogicAction desactivate;
        public Animator animator;
        public GameObject alternativeTarget;
        public Enemy[] assignedGuards;
        public bool isTargetInputHiddenFromPlayer;
        private DataGate targetsDataInput;
        private EnemySharedInfoManager enemySharedInfoManager;
        public bool trigger;

        internal override void OnStart()
        {

            var sceneManager = GameObject.FindGameObjectWithTag("SceneManager");
            var alarmManager = sceneManager.GetComponent<AlarmManager>();
            enemySharedInfoManager = sceneManager.GetComponent<EnemySharedInfoManager>();


            alarmManager.gate.Connect(this.actionGate);

            activate.outputGate.ValueHasChanged += (object sender, EventArgs args) =>
            {
                if (activate.outputGate.currentValue)
                {
                    alarmManager.gate.SetValue(true);
                }
            };

            desactivate.outputGate.ValueHasChanged += (object sender, EventArgs args) =>
            {
                if (desactivate.outputGate.currentValue)
                {
                    alarmManager.gate.SetValue(false);
                }
            };

            targetsDataInput = new DataGate(true) { 
                name = "targets",
                isHiddenFromPlayer = isTargetInputHiddenFromPlayer 
            };
            
            dataGates.Add(targetsDataInput);

            activate.outputGate.ValueHasChanged += (object sender, EventArgs args) =>
            {
                trigger = true;
            };
        }

        private void AttractClosestGuard()
        {
            var target = targetsDataInput.GetData<GameObject>().FirstOrDefault() ?? alternativeTarget;
            var minDis = float.MaxValue;
            Enemy minEnemy = null;
            
            foreach (var guard in enemySharedInfoManager.GetAllAliveEnemies())
            {
                float magnitude = (transform.position - guard.transform.position).magnitude;
                if (magnitude < minDis && guard.gameObject != target)
                {
                    minEnemy = guard;
                    minDis = magnitude;
                }
            }

            if (minEnemy != null)
            {
                minEnemy.GetComponent<HSM>()
                .TriggerEvent("noiseHeard", new EventData{{"subject", target.transform.position}});
            }
        }

        void Update()
        {
            if(trigger){
                AttractClosestGuard();
                trigger = false;
            }
            animator.SetBool("Active", outputGate.currentValue);
        }
    }
}
