using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController : MonoBehaviour
{
    public Creature creature;
    public Rigidbody2D rb;
    public float speedModifier;
    public Animator anim;
    internal Vector3 vel;
    private Vector3 dir;
    public bool dashing;
    public float dashTime;
    public float dashStart;
    public Transform mousePos;
    public float dashSpeedModifier;

    public WorkshopCameraController hackCam;
    public Follower followCam;

    public SoundManager soundManager;

    private Vector3 animDir;
    private bool paused;

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

            // hackCam.enabled = paused;
            // followCam.enabled = !paused;
        }

        if (!paused)
        {
            dir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            creature.headDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

            creature.moveState = dir.magnitude == 0 ? MoveState.Idle : MoveState.Walk;
            anim.SetInteger("Mode", (int)creature.moveState);
        }
    }

    private void FixedUpdate()
    {
        vel = dir * speedModifier * Time.fixedDeltaTime * (creature.moveState == MoveState.Dash ? dashSpeedModifier : 1);
        rb.MovePosition(rb.transform.position + vel);
    }
}
