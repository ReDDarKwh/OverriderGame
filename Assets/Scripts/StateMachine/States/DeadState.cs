
using System.Collections.Generic;
using Scripts.States;
using UnityEngine;


namespace Scripts.States
{
    public class DeadState : AbstractState
    {
        public GameObject explodeEffect;
        public GameObject deadBodyPrefab;

        // Start is called before the first frame update
        public override void StateEnter()
        {
            if ((DamageType)memory.Get<DamageType>("damageType") == DamageType.Explosion)
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

        public override void StateExit()
        {
        }

        public override void StateUpdate()
        {
        }
    }
}