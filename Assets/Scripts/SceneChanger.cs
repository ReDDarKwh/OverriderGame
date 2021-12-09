using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public int currentSceneId;

    public AsyncOperation ChangeScene(int sceneId)
    {
        return SceneManager.LoadSceneAsync(sceneId);
    }

    public Scene ChangeSceneAdditive(int sceneId)
    {
        return SceneManager.LoadScene(sceneId, new LoadSceneParameters{ loadSceneMode = LoadSceneMode.Additive });
    }

    public void ChangeSceneAdditiveUI(int sceneId)
    {
        SceneManager.LoadScene(sceneId, new LoadSceneParameters{ loadSceneMode = LoadSceneMode.Additive });
    }

    public void RemoveSceneAdditive(int sceneId)
    {
        SceneManager.UnloadSceneAsync(sceneId == -1? currentSceneId : sceneId, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
    }

}
