using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{

    public DoorController doorController;
    public OpeningAction openingAction;
    public LayerMask AILayers;
    public float doorDamage;
    public bool disableTouchLock;

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

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!disableTouchLock)
        {
            if (AILayers == (AILayers | (1 << col.collider.gameObject.layer)))
            {
                SetIsLocked();
            }
        }
    }
}
