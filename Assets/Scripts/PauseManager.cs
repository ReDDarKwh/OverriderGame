using System;
using System.Collections;
using System.Collections.Generic;
using Lowscope.Saving;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class PauseManager : MonoBehaviour
{
    public SoundManager soundManager;

    public Animator pauseFXAnim;

    public Transform PauseTextContainer;

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
            pauseFXAnim.SetBool("pause", true);
            PauseTextContainer.gameObject.SetActive(true);
        }
    }

    public void Unpause(){
        paused = Mathf.Max(0, paused - 1);

        if(paused == 0 && isPaused){
            Time.timeScale = 1.0f;
            isPaused = false;
            soundManager.PlayAll();
            pauseFXAnim.SetBool("pause", false);
            PauseTextContainer.gameObject.SetActive(false);
        }
    }

}
