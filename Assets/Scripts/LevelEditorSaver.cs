using System.Collections;
using System.Collections.Generic;
using Lowscope.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEditorSaver : MonoBehaviour
{
    public int levelNumber;

    public int levelSceneId;

    private int SlotNumber
    {
        get
        {
            return levelNumber * 2;
        }
    }

    void Start(){
        Load();
    }

    public void Load(){
        SaveMaster.ClearActiveSavedPrefabs();
        SaveMaster.SetSlot(SlotNumber, false);
        SaveMaster.SyncLoad();
    }

    public void SaveLevel(){
        SaveMaster.DeleteSave(SlotNumber);
        SaveMaster.SetSlot(SlotNumber, false);
        SaveMaster.WriteActiveSaveToDisk();
    }
}
