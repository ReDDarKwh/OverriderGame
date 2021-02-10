using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public MoveState moveState;
    public Transform body;
    public Vector3 headDir { get; set; }
    public Navigation nav;
    public Killable killable;
    public float speed;
    public float damping = 10;

    void Update()
    {
        if (!killable.dead)
        {
            if (body != null)
            {
                var step = speed * Time.deltaTime;
                var desiredRotQ = Quaternion.LookRotation(Vector3.forward, headDir) * Quaternion.Euler(0, 0, 90);
                body.rotation = Quaternion.Lerp(body.rotation, desiredRotQ, Time.deltaTime * damping);
            }

            if (nav != null)
            {
                if (moveState != MoveState.Attack)
                {
                    moveState = nav.IsMoving() ? MoveState.Walk : MoveState.Idle;
                }
            }
        }
    }

    public void AttackEnd()
    {
        moveState = MoveState.Idle;
    }

    void OnCollisionEnter2D(Collision2D col)
    {

        Debug.Log(col.collider.tag);

        if (col.collider.tag == "Door")
        {
            var door = col.collider.GetComponent<Door>();
            door.SetIsLocked();
        }
    }
}
