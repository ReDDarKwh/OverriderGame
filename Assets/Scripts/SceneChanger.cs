using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string currentSceneName;

    public AsyncOperation ChangeScene(string sceneName)
    {
        return SceneManager.LoadSceneAsync(sceneName);
    }

    public Scene ChangeSceneAdditive(string sceneName)
    {
        return SceneManager.LoadScene(sceneName, new LoadSceneParameters{ loadSceneMode = LoadSceneMode.Additive });
    }

    public void ChangeSceneAdditiveUI(string sceneName)
    {
        SceneManager.LoadScene(sceneName, new LoadSceneParameters{ loadSceneMode = LoadSceneMode.Additive });
    }

    public void RemoveSceneAdditive(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName == ""? currentSceneName : sceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
    }

}
