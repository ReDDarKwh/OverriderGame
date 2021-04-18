using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Killable : MonoBehaviour
{
    public float health;
    public bool dead;
    public event EventHandler hasDied;
    public UnityEvent OnDied;
    public DirtCreator dirt;

    public HSM stateMachine;


    internal void InflictDamage(float damage, DamageType damageType = DamageType.Unknown)
    {
        health -= damage;

        if (dirt)
        {
            dirt.Run();
        }

        if (health <= 0)
        {
            dead = true;
            if (stateMachine)
            {
                stateMachine.TriggerEvent("died", new Dictionary<string, object> { { "damageType", damageType } });
            }

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
