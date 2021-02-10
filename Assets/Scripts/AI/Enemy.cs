using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Scripts.AI;
using Bolt;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Creature creature;
    public Animator anim;
    public SpriteRenderer spriteRenderer;

    public Killable killable;


    // Update is called once per frame
    void Update()
    {
        if (!killable.dead)
        {
            anim.SetInteger("Mode", (int)creature.moveState);

            var angle = (Mathf.Atan2(creature.headDir.y, creature.headDir.x) * Mathf.Rad2Deg + 360) % 360;

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

    public void Attack()
    {
        creature.moveState = MoveState.Attack;
    }

    public void OnDead(DamageType deathDamageType)
    {
        anim.SetInteger("Dir", 1);
        anim.SetInteger("Mode", 0);
        spriteRenderer.sortingOrder = 9;
        spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, 90);
    }
}
