
using System.Collections.Generic;
using Scripts.States;
using UnityEngine;

public class DeadState : AbstractState
{
    public GameObject explodeEffect;
    public GameObject deadBodyPrefab;

    // Start is called before the first frame update
    public override void StateEnter(Dictionary<string, object> evtData)
    {
        if ((DamageType)evtData["damageType"] == DamageType.Explosion)
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