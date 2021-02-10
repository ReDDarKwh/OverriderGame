using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    public NavMeshObstacle obstacle;
    public OpeningAction openingAction;
    public float doorDamage;
    public float lockTime;
    private float lockStartTime;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if ((Time.time - lockStartTime > lockTime && obstacle.enabled) || openingAction.outputGate.currentValue)
        {
            obstacle.enabled = false;
        }
    }

    internal void SetIsLocked()
    {
        obstacle.enabled = true;
        lockStartTime = Time.time;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        var killable = collider.GetComponent<Killable>();
        if (killable != null && openingAction.IsDoorClosing())
        {
            killable.InflictDamage(doorDamage, DamageType.Explosion);
        }
    }
}
