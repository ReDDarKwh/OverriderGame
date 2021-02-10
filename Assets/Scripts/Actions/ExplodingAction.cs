using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExplodingAction : Action
{
    public Killable parentToDestroy;
    public Animator animator;

    private bool isExploded;

    internal override void OnStart()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isExploded)
        {
            return;
        }

        if (outputGate.currentValue)
        {
            animator.SetTrigger("Explode");
        }
    }

    public void Explode()
    {
        isExploded = true;
        parentToDestroy.InflictDamage(int.MaxValue, DamageType.Explosion);
    }
}
