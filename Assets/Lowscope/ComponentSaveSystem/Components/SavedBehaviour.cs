using System;
using Lowscope.Saving;
using UnityEngine;

namespace Lowscope.Saving.Components
{
    /// <summary>
    /// Example class of how to store a position.
    /// Also very useful for people looking for a simple way to store a position.
    /// </summary>

    public abstract class SavedBehaviour : MonoBehaviour, ISaveable
    {
        public bool enableLevelSave;

        public abstract void OnLoad(string data);

        public abstract string OnSave();

        public abstract bool OnSaveCondition();

        public bool OnSaveCondition(bool isLevelSave)
        {
            return (!isLevelSave || enableLevelSave) && OnSaveCondition();
        }
    }
}
