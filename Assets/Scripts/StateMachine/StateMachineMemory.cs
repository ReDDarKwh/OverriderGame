using System;
using System.Collections;
using System.Collections.Generic;
using Lowscope.Saving;
using Newtonsoft.Json;
using UnityEngine;

public class StateMachineMemory : MonoBehaviour
{
    private Dictionary<string, MemoryValue> vals = new Dictionary<string, MemoryValue>();
    private Dictionary<string, GameObject> refRepo;

    [System.Serializable]
    public class MemoryValue{
        public object Value {get;set;}
        public MemoryType MemoryType {get;set;}
    }

    void Start(){
        refRepo = GameObject.FindGameObjectWithTag("SceneManager")
        .GetComponent<EnemySharedInfoManager>().objectRepo;
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

    public void Set(string name, Vector3 value)
    {
        Set(name, value, MemoryType.Vector);
    }

    public void Set(string name, GameObject value)
    {
        Set(name, value, MemoryType.GameObject);
    }

    public void Set(string name, Component value)
    {
        Set(name, value, MemoryType.Component);
    }

    public void Set(string name, object value)
    {
        Set(name, value, MemoryType.Value);
    }

    public T Get<T>(string name, bool remove = true)
    {
        MemoryValue result;
        var found = vals.TryGetValue(name, out result);

        if(remove){
            Delete(name);
        }
        
        T r = default(T);
        try{
            r = found ? result?.Value == null? default(T) : (T)result.Value : default(T);
        } catch (Exception ex){
            throw ex;
        }

        return r;
    }

    public void Delete(string name)
    {
        vals.Remove(name);
    }

    public List<SavedMemory> OnSave()
    {
        List<SavedMemory> data = new List<SavedMemory>();

        foreach(var memItem in vals){

            string uniqueId = "";
            object val = null;
            MemoryType memtype = memItem.Value.MemoryType;

            if(memItem.Value.Value != null){
                if(memItem.Value.MemoryType == MemoryType.Component){
                    var component = (Component)memItem.Value.Value;
                    if(component != null){
                        uniqueId = component.GetComponent<UniqueId>().uniqueId + "/" + component.GetType().Name;
                    }
                } else if(memItem.Value.MemoryType == MemoryType.GameObject){
                    var gameObject = (GameObject)memItem.Value.Value;
                    if(gameObject != null){
                        uniqueId = gameObject.GetComponent<UniqueId>().uniqueId;
                    }
                } else {
                    if(memItem.Value.MemoryType == MemoryType.Vector || memItem.Value.Value is Vector3){
                        var vec = (Vector3)memItem.Value.Value;
                        val = new SavedVec{x = vec.x, y = vec.y, z = vec.z};
                        memtype = MemoryType.Vector;
                    } else {
                        val = memItem.Value.Value;
                    }
                }
            }

            data.Add(new SavedMemory{
                key = memItem.Key, 
                value = val, 
                uniqueId = uniqueId,
                memType = memtype
            });
        }

        return data;
    }

    public void OnLoad(List<SavedMemory> savedData)
    {
        vals.Clear();

        foreach(var sd in savedData){
            object val = null;
            if(!string.IsNullOrEmpty(sd.uniqueId))
            {
                val = GetRef(sd);
            }
            else {
                if(sd.memType == MemoryType.Vector){
                    var vec = (dynamic)sd.value;
                    val = new Vector3((float)vec.x, (float)vec.y, (float)vec.z);
                } else {
                    val = sd.value;
                }
            }

            Set(sd.key, val, sd.memType);
        }
    }

    private object GetRef(SavedMemory sd)
    {
        object val;
        var uniqueIdPath = sd.uniqueId.Split('/');
        var gameObjectId = uniqueIdPath[0];
        var componentId = "";

        if (uniqueIdPath.Length > 1)
        {
            componentId = uniqueIdPath[1];
        }

        GameObject gameObject = null;

        if(refRepo == null){
            Start();
        }

        refRepo.TryGetValue(gameObjectId, out gameObject);

        val = gameObject;

        if (!string.IsNullOrEmpty(componentId))
        {
            val = gameObject?.GetComponent(componentId);
        }

        return val;
    }

    public bool OnSaveCondition()
    {
        return true;
    }
}