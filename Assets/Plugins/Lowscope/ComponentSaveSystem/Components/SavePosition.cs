using System;
using Lowscope.Saving;
using UnityEngine;

namespace Lowscope.Saving.Components
{
    /// <summary>
    /// Example class of how to store a position.
    /// Also very useful for people looking for a simple way to store a position.
    /// </summary>

    [AddComponentMenu("Saving/Components/Save Position"), DisallowMultipleComponent]
    public class SavePosition : MonoBehaviour, ISaveable
    {
        Vector3 lastPosition;

        [Serializable]
        public struct SaveData
        {
            public Vector3 position;
        }

        public void OnLoad(string data)
        {
            var pos = JsonUtility.FromJson<SaveData>(data).position;
            transform.position = pos;
            lastPosition = pos;
        }

        public string OnSave()
        {
            try{
                lastPosition = transform.position;
                return JsonUtility.ToJson(new SaveData { position = lastPosition });
            }
            catch (MissingReferenceException){
                return "";
            }
        }

        public bool OnSaveCondition()
        {
            return this != null && this.gameObject.activeSelf && lastPosition != transform.position;
        }
    }
}
