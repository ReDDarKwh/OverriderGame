using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Hacking;
using UnityEngine;

public class AlarmManager : MonoBehaviour
{
    internal OrGate gate = new OrGate();
    public SoundManager soundManager;
    public GameObject soundPrefab;
    public float alarmTime;

    private float alarmStartTime;
    private AudioSource audioSource;

    void Start()
    {
        gate.ValueHasChanged += (object sender, EventArgs args) =>
        {
            if(gate.currentValue){
                audioSource = soundManager.Play(soundPrefab, Vector3.zero);
                alarmStartTime = Time.time;
            } else {
                soundManager.Stop(audioSource);
                audioSource = null;
            }
        }; 
    }

    void Update(){
        
        if(gate.currentValue &&  Time.time - alarmStartTime > alarmTime){
            gate.SetValue(false);
        }
    }
}
