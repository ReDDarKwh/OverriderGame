using System;
using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public event EventHandler playerHasEntered;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            OnEnter(EventArgs.Empty);
        }
    }

    protected virtual void OnEnter(EventArgs e)
    {
        EventHandler handler = playerHasEntered;
        handler?.Invoke(this, e);
    }
}