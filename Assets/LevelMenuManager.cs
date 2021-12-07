using System.Collections;
using System.Collections.Generic;
using Lowscope.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenuManager : MonoBehaviour
{
    public SoundManager soundManager;
    public SceneChanger sceneChanger;
    public PauseManager pauseManager;
    public int pauseMenuSceneId;
    public int menuSceneId;

    [System.NonSerialized]
    public bool isPauseMenuDisplayed;

    private Scene menuScene;

     void Awake(){
        
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void Update()
    {
        if (!isPauseMenuDisplayed && Input.GetKeyDown(KeyCode.Escape))
        {
            ShowPauseMenu();
        } else if (Input.GetKeyDown(KeyCode.Escape)) {

            //HidePauseMenu();
        }
    }

    private void ShowPauseMenu()
    {
        pauseManager.Pause();
        menuScene = sceneChanger.ChangeSceneAdditive(menuSceneId);
        sceneChanger.ChangeSceneAdditive(pauseMenuSceneId);
        isPauseMenuDisplayed = true;
    }

    private void OnSceneUnloaded(Scene arg0)
    {
        if(arg0 == menuScene){
            pauseManager.Unpause();
            isPauseMenuDisplayed = false;
        }
    }

    void OnDestroy(){
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
}
