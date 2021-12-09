using System.Collections;
using System.Collections.Generic;
using Lowscope.Saving;
using UnityEngine;

public class GameOverMenuUIController : MonoBehaviour
{
    public SceneChanger sceneChanger;

    public void Retry(){
        LevelEditorSaver.Instance.ResetLevel();
    }
  
}
