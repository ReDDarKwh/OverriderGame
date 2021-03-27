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
    public Collider2D ignoredCollider;

    public float shootOffset;
    public float shootSpeed;
    public bool isDelayedShot;

    private float time;
    private bool canShoot;

    void Update()
    {
        if (shootingAction.outputGate.currentValue)
        {
            time += Time.deltaTime * shootSpeed;
            if (Mathf.Abs(Mathf.Sin(time + shootOffset)) >= 0.5 && canShoot)
            {
                StartShoot();
                canShoot = false;
            };

            if (Mathf.Abs(Mathf.Sin(time + shootOffset)) < 0.5)
            {
                canShoot = true;
            };
        }
    }
    private void StartShoot()
    {
        if (animator)
            animator.SetTrigger("Shoot");

        if (!isDelayedShot)
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        var projectile = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
        projectile.GetComponent<NoiseEmitter>().ignoredColliders = new Collider2D[] { ignoredCollider };

        SoundManager.Instance.Make(shootingSound, transform.position);
        particle.Play();
    }
}
