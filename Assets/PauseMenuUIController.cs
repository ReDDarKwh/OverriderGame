using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuUIController : MonoBehaviour
{
    public void RestartMission(){
        LevelEditorSaver.Instance.ResetLevel();
    }
}
