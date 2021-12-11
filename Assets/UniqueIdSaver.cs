using System;
using System.Collections;
using System.Collections.Generic;
using Lowscope.Saving;
using UnityEngine;

public class UniqueIdSaver : MonoBehaviour, ISaveable
{
    public UniqueId uniqueId;

    [Serializable]
    public struct SaveData
    {
        public string id;
    }

    public void OnLoad(string data)
    {
        uniqueId.uniqueId = JsonUtility.FromJson<SaveData>(data).id;
    }

    public string OnSave()
    {
        return JsonUtility.ToJson(new SaveData { id = uniqueId.uniqueId });
    }

    public bool OnSaveCondition()
    {
        return true;
    }
}
