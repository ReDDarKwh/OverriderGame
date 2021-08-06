using System.Collections;
using System.Collections.Generic;
using Bolt;
using Lowscope.Saving;
using UnityEngine;

public class StateMachineMemory : MonoBehaviour
{
    private Dictionary<string, object> vals = new Dictionary<string, object>();

    public void Set(string name, object value)
    {
        if (vals.ContainsKey(name))
        {
            vals[name] = value;
        }
        else
        {
            vals.Add(name, value);
        }
    }

    public T Get<T>(string name, bool remove = true)
    {
        object result;
        var found = vals.TryGetValue(name, out result);
        return found ? (T)result : default(T);
    }

    public void Delete(string name)
    {
        if (vals.ContainsKey(name))
        {
            vals.Remove(name);
        }
    }

}