using System;
using System.Collections;
using System.Collections.Generic;
using Lowscope.Saving;
using UnityEngine;

public class SwitchAction : Scripts.Actions.Action, ISaveable
{
    public float onTime;
    private float lastOnTime;

    [Serializable]
    public struct SaveData
    {
        public bool isOn;
        public float timeOn;
    }

    public void OnLoad(string data)
    {
        base.OnLoad(data);
        var sd = JsonUtility.FromJson<SaveData>(data);
        actionGate.SetValue(sd.isOn);
        lastOnTime = Time.time - sd.timeOn;
    }

    public string OnSave()
    {
        return JsonUtility.ToJson(new SaveData {
                isOn = actionGate.currentValue,
                timeOn = Time.time - lastOnTime
            });
    }

    public bool OnSaveCondition()
    {
        return true;
    }

    public void Toggle()
    {
        actionGate.SetValue(!actionGate.currentValue);

        if (actionGate.currentValue)
        {
            lastOnTime = Time.time;
        }
    }

    internal override void OnStart()
    {
    }
    
    void Update()
    {
        if (Time.time - lastOnTime > onTime && actionGate.currentValue)
        {
            actionGate.SetValue(false);
        }
    }
}
