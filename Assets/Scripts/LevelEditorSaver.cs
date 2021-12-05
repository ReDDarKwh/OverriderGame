using System.Collections;
using System.Collections.Generic;
using Lowscope.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEditorSaver : MonoBehaviour
{
    public int levelNumber;

    private int SlotNumber
    {
        get
        {
            return levelNumber * 2;
        }
    }

    void Start(){
        //SaveMaster.SpawnInstanceManager(SceneManager.GetActiveScene());
        SaveMaster.SetSlot(SlotNumber, true);
    }

    public void SaveLevel(){
        SaveMaster.SetSlot(SlotNumber, false);
        SaveMaster.WriteActiveSaveToDisk();
    }
}
