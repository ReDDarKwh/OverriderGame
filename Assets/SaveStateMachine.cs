using System.Collections;
using System.Collections.Generic;
using Bolt;
using Lowscope.Saving;
using UnityEngine;
using System.Linq;

public class SaveStateMachine : MonoBehaviour, ISaveable
{
    public void OnLoad(string data)
    {
        var declarations = JsonUtility.FromJson<Dictionary<string, object>>(data);
        foreach (var d in declarations)
        {
            Variables.Object(this.gameObject).Set(d.Key, d.Value);
        }

        CustomEvent.Trigger(gameObject, "reset");
    }

    public string OnSave()
    {
        return JsonUtility.ToJson(Variables.Object(gameObject).ToDictionary(k => k.name, v => v.value));
    }

    public bool OnSaveCondition()
    {
        return true;
    }
}
