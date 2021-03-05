using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void ChangeScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }

    public void ChangeSceneAdditive(int sceneId)
    {
        SceneManager.LoadScene(sceneId, LoadSceneMode.Additive);
    }
}
