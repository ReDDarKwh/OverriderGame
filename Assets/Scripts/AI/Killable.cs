using System;
using System.Collections;
using System.Collections.Generic;
using Lowscope.Saving;
using UnityEngine;
using UnityEngine.Events;

public class Killable : MonoBehaviour, ISaveable
{
    public float health;
    public bool dead;
    public event EventHandler hasDied;
    public UnityEvent OnDied;
    public DirtCreator dirt;

    public HSM stateMachine;

    [Serializable]
    public struct SaveData
    {
        public bool dead;
        public float health;
    }

    public void InflictDamage(float damage, DamageType damageType = DamageType.Unknown)
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
                stateMachine.memory.Set("damageType", damageType, MemoryType.Value);
                stateMachine.TriggerEvent("died");
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

    public string OnSave()
    {
        return JsonUtility.ToJson(new SaveData { dead = dead, health = health });
    }

    public void OnLoad(string data)
    {
        var sd = JsonUtility.FromJson<SaveData>(data);
        dead = sd.dead;
        health = sd.health;  

        if(!dead){
            this.gameObject.SetActive(true);
        }
    }

    public bool OnSaveCondition(bool v)
    {
        return true;
    }
}
