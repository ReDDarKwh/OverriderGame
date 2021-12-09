using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverMenuUIController : MonoBehaviour
{
    public SceneChanger sceneChanger;

    public void Retry(){
        StartCoroutine(LoadYourAsyncScene());
    }

    IEnumerator LoadYourAsyncScene()
    {
        AsyncOperation asyncLoad = sceneChanger.ChangeScene(GameObject.FindGameObjectWithTag("SceneManager")
        .GetComponent<LevelEditorSaver>().levelSceneId);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        GameObject.FindGameObjectWithTag("SceneManager")
            .GetComponent<LevelEditorSaver>().Load();
    }
}
