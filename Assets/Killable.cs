using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class Killable : MonoBehaviour
{
    public float health;
    public bool dead;
    public event EventHandler hasDied;

    internal void InflictDamage(float damage, DamageType damageType = DamageType.Unknown)
    {
        health -= damage;
        Debug.Log($"{this.gameObject.name} took {damage} damage");

        if (health <= 0 && !dead)
        {
            dead = true;
            CustomEvent.Trigger(gameObject, "Died", damageType);
            OnDeath(EventArgs.Empty);
        }
    }

    protected virtual void OnDeath(EventArgs e)
    {
        EventHandler handler = hasDied;
        handler?.Invoke(this, e);
    }
}
