using System.Collections;
using System.Collections.Generic;
using Lowscope.Saving;
using UnityEngine;

public class ParentSetter : MonoBehaviour, ISaveable
{
    public string tagName;

    public void OnLoad(string data)
    {
        transform.SetParent(GameObject.FindGameObjectWithTag(tagName).transform);
    }

    public string OnSave()
    {
        return "x";
    }

    public bool OnSaveCondition(bool v)
    {
        return true;
    }
}
