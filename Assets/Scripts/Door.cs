using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{

    public DoorController doorController;
    public OpeningAction openingAction;
    public float doorDamage;

    internal void SetIsLocked()
    {
        doorController.SetIsLocked();
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
