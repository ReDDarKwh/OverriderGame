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

    void Update()
    {
        if (!killable.dead)
        {
            anim.SetInteger("Mode", (int)creature.moveState);
        }
    }

    public void OnDead(DamageType deathDamageType)
    {
        anim.SetInteger("Dir", 1);
        anim.SetInteger("Mode", 0);
        spriteRenderer.sortingOrder = 9;
        spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, 90);
    }
}
