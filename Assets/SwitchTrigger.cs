﻿using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class SwitchTrigger : MonoBehaviour
{
    public Interactable interactable;
    public DoorController doorController;

    void Start()
    {
        interactable.onUsed.AddListener(OnSwitchUsed);
    }

    void OnSwitchUsed()
    {
        //doorController.openingAction
        StartCoroutine("CheckDoor");
    }

    IEnumerator CheckDoor()
    {
        yield return new WaitForSeconds(0.5f);

        if (!doorController.openingAction.outputGate.currentValue)
        {
            doorController.SetIsLocked();
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        var creature = collider.GetComponent<Creature>();
        if (creature != null)
        {
            CustomEvent.Trigger(collider.gameObject, "ActivateSwitchRequested", interactable.gameObject, doorController);
        }
    }
}
