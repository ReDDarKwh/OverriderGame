using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController : MonoBehaviour
{
    public Creature creature;
    public Rigidbody2D rb;
    public float speedModifier;
    public Animator anim;
    private Vector3 vel;
    private Vector3 dir;
    public bool dashing;
    public float dashTime;
    public float dashStart;
    public float dashSpeedModifier;

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
            }
            else
            {
                Time.timeScale = 1.0f;
                paused = false;
            }
        }

        if (!paused)
        {
            if (creature.moveState != MoveState.Dash)
            {
                dir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            }

            animDir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized.magnitude == 0 ? animDir : new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

            // if (Input.GetButtonDown("Dash") && creature.moveState != MoveState.Dash && dir.magnitude != 0)
            // {
            //     dashStart = Time.time;
            //     creature.moveState = MoveState.Dash;
            // }

            if (Time.time >= dashStart + dashTime && creature.moveState == MoveState.Dash)
            {
                creature.moveState = MoveState.Idle;
            }

            if (creature.moveState != MoveState.Dash && creature.moveState != MoveState.Stab)
            {
                creature.moveState = dir.magnitude == 0 ? MoveState.Idle : MoveState.Walk;

                if (Input.GetButtonDown("Stab"))
                {
                    creature.moveState = MoveState.Stab;
                }
            }

            anim.SetInteger("Mode", (int)creature.moveState);

            if (Input.GetButton("Horizontal") && Input.GetAxisRaw("Horizontal") > 0)
            {
                anim.SetInteger("Dir", 0);
            }
            else if (Input.GetButton("Horizontal") && Input.GetAxisRaw("Horizontal") < 0)
            {
                anim.SetInteger("Dir", 2);
            }
            if (Input.GetButton("Vertical") && Input.GetAxisRaw("Vertical") > 0)
            {
                anim.SetInteger("Dir", 1);
            }
            else if (Input.GetButton("Vertical") && Input.GetAxisRaw("Vertical") < 0)
            {
                anim.SetInteger("Dir", 3);
            }

            if (Input.GetButtonUp("Vertical") || Input.GetButtonUp("Horizontal"))
            {
                var angle = Mathf.Atan2(animDir.y, animDir.x) * Mathf.Rad2Deg;
                if ((angle >= 0 && angle < 45) || (angle < 360 && angle > 315))
                {
                    anim.SetInteger("Dir", 0);
                }
                else if (angle >= 45 && angle < 135)
                {
                    anim.SetInteger("Dir", 1);
                }
                else if (angle >= 135 && angle < 225)
                {
                    anim.SetInteger("Dir", 2);
                }
                else
                {
                    anim.SetInteger("Dir", 3);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        vel = dir * speedModifier * Time.fixedDeltaTime * (creature.moveState == MoveState.Dash ? dashSpeedModifier : 1);
        rb.MovePosition(rb.transform.position + vel);
    }
}
