using System.Collections;
using System.Collections.Generic;
using Bolt;
using Lowscope.Saving;
using Newtonsoft.Json;
using UnityEngine;

public class StateMachineMemory : MonoBehaviour, ISaveable
{
    private Dictionary<string, MemoryValue> vals = new Dictionary<string, MemoryValue>();

    [System.Serializable]
    public class MemoryValue{
        public object Value {get;set;}
        public MemoryType MemoryType {get;set;}
    }

    public void Set(string name, object value, MemoryType memType)
    {
        var v = new MemoryValue{Value = value, MemoryType = memType};

        if (vals.ContainsKey(name))
        {
            vals[name] = v;
        }
        else
        {
            vals.Add(name, v);
        }
    }

    public T Get<T>(string name, bool remove = true)
    {
        MemoryValue result;
        var found = vals.TryGetValue(name, out result);

        if(remove){
            Delete(name);
        }

        return found ? (T)result.Value : default(T);
    }

    public void Delete(string name)
    {
        vals.Remove(name);
    }

    public string OnSave()
    {
        List<SavedMemory> data = new List<SavedMemory>();

        foreach(var memItem in vals){

            string uniqueId = "";
            object val = null;

            if(memItem.Value.Value != null){
                if(memItem.Value.MemoryType == MemoryType.Component){
                    var component = (Component)memItem.Value.Value;
                    uniqueId = component.GetComponent<UniqueId>().uniqueId + "/" + component.name;
                } else if(memItem.Value.MemoryType == MemoryType.GameObject){
                    var gameObject = (GameObject)memItem.Value.Value;
                    uniqueId = gameObject.GetComponent<UniqueId>().uniqueId;
                } else {
                    if(memItem.Value.Value is Vector3){
                        var vec = (Vector3)memItem.Value.Value;
                        val = new SavedVec{x = vec.x, y = vec.y, z = vec.z};
                    } else {
                        val = memItem.Value.Value;
                    }
                }
            }

            data.Add(new SavedMemory{key = memItem.Key, value = val, uniqueId = uniqueId});
        }

        var str = JsonConvert.SerializeObject(data);
        
        Debug.Log(str);

        return str;
    }

    public void OnLoad(string data)
    {
        
    }

    public bool OnSaveCondition()
    {
        return true;
    }
}