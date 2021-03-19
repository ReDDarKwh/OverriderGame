using System.Collections;
using System.Collections.Generic;
using Scripts.Actions;
using UnityEngine;

namespace Scripts.StateMachine.States
{
    public class DeadState : BaseState
    {
        void Awake()
        {
            stateName = "DeadState";
        }

        public GameObject explodeEffect;
        public GameObject deadBodyPrefab;

        public override void Enter(StateMachine stateMachine, EventMessage eventResponse, ActiveLinking activeLinking)
        {
            if (activeLinking.GetValueOrDefault<DamageType>("damageType") == DamageType.Explosion)
            {
                Instantiate(explodeEffect, transform.position, Quaternion.identity);
                Destroy(this.gameObject);
            }
            else
            {
                Instantiate(deadBodyPrefab, transform.position, Quaternion.identity);
                Destroy(this.gameObject);
            }
        }

        public override void Leave(StateMachine stateMachine)
        {
        }

        public override void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking)
        {
        }
    }
}

