using System;
using System.Collections;
using System.Collections.Generic;
using Lowscope.Saving;
using Scripts.Actions;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    public DoorController doorController;
    public OpeningAction openingAction;
    public LayerMask AILayers;
    public float doorDamage;
    private bool disableTouchLock;

    void Awake(){
        SaveMaster.OnSlotChangeBegin += OnSlotChangeBegin;
        SaveMaster.OnSlotChangeDone += OnSlotChangeDone;
    }

    private void OnSlotChangeDone(int obj)
    {
        disableTouchLock = false;  
    }

    private void OnSlotChangeBegin(int obj)
    {
        disableTouchLock = true;        
    }

    void OnDestroy(){
        SaveMaster.OnSlotChangeBegin -= OnSlotChangeBegin;
        SaveMaster.OnSlotChangeDone -= OnSlotChangeDone;
    }

    internal void SetIsLocked(Creature creature)
    {
        doorController.SetIsLocked(creature);
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
                SetIsLocked(col.gameObject.GetComponent<Creature>());
            }
        }
    }
}
