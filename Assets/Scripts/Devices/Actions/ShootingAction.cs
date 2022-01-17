using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Actions
{
    public class ShootingAction : Action
    {
        public GameObject projectilePrefab;
        public Transform barrelPosition;
        public Collider2D ignoredCollider;

        public float shootInterval;
        private float lastShot;
        private float time;

        internal override void OnStart()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (outputGate.currentValue)
            {
                time += Time.deltaTime;
                if (time - lastShot > shootInterval)
                {
                    lastShot = time;
                    Shoot();
                }
            }
            else
            {
                lastShot = float.MinValue;
            }
        }

        private void Shoot()
        {
            var projectile = Instantiate(projectilePrefab, barrelPosition.position, transform.rotation);
            projectile.GetComponent<NoiseEmitter>().ignoredColliders = new Collider2D[] { ignoredCollider };
        }
    }
}