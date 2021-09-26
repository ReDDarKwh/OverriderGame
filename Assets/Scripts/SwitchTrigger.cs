using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTrigger : MonoBehaviour
{
    public Interactable interactable;
    public DoorController doorController;

    void Start()
    {
        if (interactable)
        {
            interactable.onUsedWithCreature.AddListener(OnSwitchUsed);
        }
    }

    void OnSwitchUsed(Creature creature)
    {
        //doorController.openingAction
        StartCoroutine(CheckDoor(creature));
    }

    IEnumerator CheckDoor(Creature creature)
    {
        yield return new WaitForSeconds(0.5f);

        if (!doorController.openingAction.outputGate.currentValue)
        {
            doorController.SetIsLocked(creature);
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (!interactable)
            return;

        var hsm = collider.GetComponent<HSM>();
        if (hsm)
        {
            hsm.TriggerEvent("activateSwitchRequested", new EventData{{"switch", interactable.gameObject}, {"doorController", doorController}});
        }
    }
}
