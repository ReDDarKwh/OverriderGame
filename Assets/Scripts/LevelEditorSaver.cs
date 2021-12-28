using System;
using System.Collections;
using System.Collections.Generic;
using Lowscope.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEditorSaver : MonoBehaviour
{
    public int levelNumber;

    public string levelSceneName;

    public static LevelEditorSaver Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    internal void ResetLevel()
    {
        StartCoroutine(LoadYourAsyncScene());
    }

    IEnumerator LoadYourAsyncScene()
    {
        SaveMaster.ClearSlot(false);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelSceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        yield return 0;
        Load();
    }

    private int SlotNumber
    {
        get
        {
            return levelNumber * 2;
        }
    }

    IEnumerator Start(){
        yield return 0;
        yield return 0;
        Load();
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.F7)){
            SaveLevel();
        }
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
