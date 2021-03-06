using System.Collections;
using System.Collections.Generic;
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

    internal Vector3 vel;
    private Vector3 dir;
    private float lastEmission;
    private bool paused;
    private GameObject[] covers;


    void Start()
    {
        covers = GameObject.FindGameObjectsWithTag("Cover");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!paused)
            {
                Time.timeScale = 0.0f;
                paused = true;
                soundManager.PauseAll();
            }
            else
            {
                Time.timeScale = 1.0f;
                paused = false;
                soundManager.PlayAll();
            }
        }

        if (Time.time - lastEmission > runNoiseEmissionInterval)
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


        if (!paused)
        {
            dir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            creature.headDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

            creature.moveState = dir.magnitude == 0 ? MoveState.Idle : Input.GetAxisRaw("Run") == 1 ? MoveState.Run : MoveState.Walk;
            anim.SetInteger("Mode", (int)creature.moveState);
        }
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
