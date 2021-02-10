using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;
using UnityEngine.Events;

public class Killable : MonoBehaviour
{
    public float health;
    public bool dead;
    public event EventHandler hasDied;
    public UnityEvent OnDied;

    internal void InflictDamage(float damage, DamageType damageType = DamageType.Unknown)
    {
        health -= damage;
        Debug.Log($"{this.gameObject.name} took {damage} damage");

        if (health <= 0 && !dead)
        {
            dead = true;
            CustomEvent.Trigger(gameObject, "Died", damageType);
            OnDied.Invoke();
            OnDeath(EventArgs.Empty);
        }
    }

    protected virtual void OnDeath(EventArgs e)
    {
        EventHandler handler = hasDied;
        handler?.Invoke(this, e);
    }
}
