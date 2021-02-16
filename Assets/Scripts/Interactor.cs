using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactor : MonoBehaviour
{
    public Animator animator;
    public void InteractWith(GameObject interactable)
    {
        var i = interactable.GetComponent<Interactable>();
        if (i != null)
        {
            if (animator != null)
            {
                animator.SetTrigger("Interact");
            }
            i.Use();
        }
    }
}
