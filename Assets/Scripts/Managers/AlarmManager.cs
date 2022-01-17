using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Hacking;
using UnityEngine;

public class AlarmManager : MonoBehaviour
{
    internal OrGate gate = new OrGate();

    public Node node;
    public SoundManager soundManager;
    public SoundPreset alarmSound;
    public float alarmTime;

    public bool trigger;

    private float alarmStartTime;
    private AudioSource audioSource;

    void Start()
    {
        gate.ValueHasChanged += (object sender, EventArgs args) =>
        {
            if (gate.currentValue)
            {
                audioSource = SoundManager.Instance.Make(alarmSound, Vector3.zero);
                alarmStartTime = Time.time;
            }
            else
            {
                SoundManager.Instance.Stop(audioSource);
                audioSource = null;
            }
        };
    }

    void Update()
    {

        if(trigger){
            gate.SetValue(true);
            trigger = false;
        }

        if (gate.currentValue && Time.time - alarmStartTime > alarmTime)
        {
            gate.SetValue(false);
        }
    }
}
