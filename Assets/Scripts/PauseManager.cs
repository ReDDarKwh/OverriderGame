using System;
using System.Collections;
using System.Collections.Generic;
using Lowscope.Saving;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public SoundManager soundManager;
    private int paused;

    [System.NonSerialized]
    public bool isPaused;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Pause(){
        paused ++;
        if(!isPaused){
            Time.timeScale = 0.0f;
            isPaused = true;
            soundManager.PauseAll();
        }
    }

    public void Unpause(){
        paused = Mathf.Max(0, paused - 1);

        if(paused == 0 && isPaused){
            Time.timeScale = 1.0f;
            isPaused = false;
            soundManager.PlayAll();
        }
    }

}
