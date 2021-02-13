using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactor : MonoBehaviour
{
    public void InteractWith(GameObject interactable)
    {
        var i = interactable.GetComponent<Interactable>();
        if (i != null)
        {
            i.Use();
        }
    }
}
