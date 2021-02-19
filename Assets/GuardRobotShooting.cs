using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardRobotShooting : MonoBehaviour
{
    public Transform muzzle;
    public Animator animator;
    public ExternalLogicAction shootingAction;
    public GameObject projectilePrefab;
    public SoundPreset shootingSound;
    public ParticleSystem particle;

    public float shootOffset;
    public float shootSpeed;

    private float time;
    private bool canShoot;

    void Update()
    {
        if (shootingAction.outputGate.currentValue)
        {
            time += Time.deltaTime * shootSpeed;
            if (Mathf.Abs(Mathf.Sin(time + shootOffset)) >= 0.9 && canShoot)
            {
                Shoot();
                canShoot = false;
            };

            if (Mathf.Abs(Mathf.Sin(time + shootOffset)) <= 0.1)
            {
                canShoot = true;
            };
        }
    }
    private void Shoot()
    {
        var projectile = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
        animator.SetTrigger("Shoot");
        SoundManager.Instance.Make(shootingSound, transform.position);
        particle.Play();
    }
}
