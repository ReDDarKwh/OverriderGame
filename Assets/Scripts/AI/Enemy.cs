using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Scripts.AI;
using UnityEngine.AI;
using Scripts.Actions;

public class Enemy : MonoBehaviour
{
    public Creature creature;
    public Animator anim;
    public SpriteRenderer spriteRenderer;
    public Killable killable;
    public HSM hsm;
    public bool isHostile;

    public float alertSpottingRadius;
    public int alertSpottingAngle;
    public float alertUnSureTime;

    private bool alert;
    private AlarmManager alarm;
    public SpottingAction spotting;
    

    void Start()
    {
        alarm = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<AlarmManager>();

        alarm.gate.ValueHasChanged += (object sender, EventArgs args) =>
        {
            if (alarm.gate.currentValue)
            {
                SetAlert();
                hsm.TriggerEvent("alarmTriggered");
            }
        };
    }

    void SetAlert()
    {
        if (!alert)
        {
            spotting.VisionRadius = alertSpottingRadius;
            spotting.VisionAngle = alertSpottingAngle;
            alert = true;
        }
    }

    void Update()
    {
        if (!killable.dead)
        {
            anim.SetInteger("Mode", (int)creature.moveState);
        }
    }

    public void OnDead()
    {
        anim.SetInteger("Mode", 0);
        spriteRenderer.sortingOrder = 9;
        spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, 90);
    }
}
