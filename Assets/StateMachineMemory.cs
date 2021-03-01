using System.Collections;
using System.Collections.Generic;
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

    public object Get(string name, bool remove = true)
    {
        if (!vals.ContainsKey(name))
        {
            return null;
        }

        var val = vals[name];

        if (remove)
        {
            vals.Remove(name);
        }
        return val;
    }

    public void Delete(string name)
    {
        if (vals.ContainsKey(name))
        {
            vals.Remove(name);
        }
    }
}