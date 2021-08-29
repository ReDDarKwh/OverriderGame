using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactor : MonoBehaviour
{
    public Animator animator;
    public Creature creature;
    
    public void InteractWith(Interactable interactable)
    {
        var i = interactable;
        if (i != null)
        {
            if (creature)
            {
                creature.headDir = interactable.transform.position - transform.position;
            }
            if (animator != null)
            {
                animator.SetTrigger("Interact");
            }
            i.Use();
        }
    }
}
