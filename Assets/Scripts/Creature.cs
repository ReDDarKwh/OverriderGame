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
    public float damping = 7;

    [System.NonSerialized]
    public bool canLock;

    void Start()
    {
        headDir = body.rotation * Vector3.right;
    }

    void Update()
    {
        if (!killable.dead)
        {
            if (body != null)
            {
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

    public void MakeSound(SoundPreset sound)
    {
        SoundManager.Instance.Make(sound, transform.position);
    }

}
