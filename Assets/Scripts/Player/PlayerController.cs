using System;
using System.Collections;
using System.Collections.Generic;
using Lowscope.Saving;
using UnityEngine;

public partial class PlayerController : MonoBehaviour
{
    public Creature creature;
    public Rigidbody2D rb;
    public float speedModifier;
    public float runSpeedModifier;
    public NoiseEmitter runNoiseEmitter;
    public NoiseEmitter sneakNoiseEmitter;
    public float runNoiseEmissionInterval;
    public Animator anim;
    public SoundManager soundManager;
    public string lowCoverLayerMask;
    public string coverLayerMask;
    public PauseManager pauseManager;
    public LevelMenuManager levelMenuManager;
    
    internal Vector3 vel;
    private Vector3 dir;
    private float lastEmission;
    private GameObject[] covers;
    
    void Start()
    {
        covers = GameObject.FindGameObjectsWithTag("Cover");
    }

    // Update is called once per frame
    void Update()
    {
        if(levelMenuManager.isPauseMenuDisplayed){
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!pauseManager.isPaused)
            {
                pauseManager.Pause();
            }
            else
            {
                pauseManager.Unpause();
            }
        }

        if (pauseManager.isPaused){
            return;
        }

        if (creature.moveState != MoveState.Idle && Time.time - lastEmission > runNoiseEmissionInterval)
        {
            lastEmission = Time.time;
            if (Input.GetAxisRaw("Run") == 1)
            {
                runNoiseEmitter.EmitNoise();
            }
            else
            {
                sneakNoiseEmitter.EmitNoise();
            }
        }

        if (Input.GetButtonDown("Run"))
        {
            ChangeCover(true);
        }

        if (Input.GetButtonUp("Run"))
        {
            ChangeCover(false);
        }

        dir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        creature.headDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        creature.moveState = dir.magnitude == 0 ? MoveState.Idle : Input.GetAxisRaw("Run") == 1 ? MoveState.Run : MoveState.Walk;
    }

    private void ChangeCover(bool lower)
    {
        foreach (var cover in covers)
        {
            cover.layer = lower ? LayerMask.NameToLayer(lowCoverLayerMask) : LayerMask.NameToLayer(coverLayerMask);
        }
    }

    private void FixedUpdate()
    {
        vel = dir * speedModifier * Time.fixedDeltaTime * (creature.moveState == MoveState.Run ? runSpeedModifier : 1);
        rb.MovePosition(rb.transform.position + vel);
    }
}
