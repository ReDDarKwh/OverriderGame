using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEditor;
using UnityEngine.AI;

namespace Scripts.AI
{
    public class DirectNav : MonoBehaviour
    {

        public float neighborhoodRadius;
        public float mass;
        public float maxSteeringForce;
        public float maxSteeringSpeed;
        public string[] separationLayers = new String[] { "AI", "Player" };
        private HashSet<GameObject> ignoredSeparationNeighbors = new HashSet<GameObject>();

        [System.NonSerialized]
        public bool separateFromPlayer = true;

        public bool NeightborAvoidance
        {
            set
            {
                this.neightborAvoidance = value;
            }
        }

        public float Speed
        {
            set
            {
                this.speed = value;
            }
        }

        public bool isWaitingForPath { get; private set; }

        public bool isMoving { get; private set; }

        public float targetsRadius;
        public bool neightborAvoidance = true;
        public float speed;

        private ConcurrentStack<Vector3> path;
        private Transform currentTarget;
        private float startTime;
        private Vector3 startPos;
        private Rigidbody2D rb;
        private Vector3 vel;


        // Use this for initialization
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            path = new ConcurrentStack<Vector3>();
        }

        // Update is called once per frame
        void Update()
        {
            if (isMoving)
            {
                if ((transform.position - currentTarget.position).magnitude < targetsRadius)
                {
                    this.isMoving = false;
                }
            }
        }

        void FixedUpdate()
        {
            Vector3 desiredVelocity = ((isMoving ? (currentTarget.position - transform.position).normalized : Vector3.zero) +
             ComputeSeparation()) * Time.fixedDeltaTime * speed;

            var sterring = Vector3.ClampMagnitude((desiredVelocity - vel), maxSteeringForce) / mass;
            vel = Vector2.ClampMagnitude(vel + sterring, maxSteeringSpeed);

            rb.MovePosition(rb.position + (Vector2)vel);
        }

        public void AddSeparationIgnored(GameObject obj)
        {
            if (!ignoredSeparationNeighbors.Contains(obj))
                ignoredSeparationNeighbors.Add(obj);
        }

        public void RemoveSeparationIgnored(GameObject obj)
        {
            ignoredSeparationNeighbors.Remove(obj);
        }

        private Vector3 ComputeSeparation()
        {
            var resultVec = Vector3.zero;

            if (neightborAvoidance)
            {
                var neighbors = Physics2D.OverlapCircleAll(
                    transform.position,
                    neighborhoodRadius,
                    LayerMask.GetMask(separationLayers)
                );

                foreach (var n in neighbors)
                {
                    if (!ignoredSeparationNeighbors.Contains(n.gameObject))
                        resultVec += n.transform.position - transform.position;
                }

                resultVec *= -1;
            }

            return resultVec.normalized;
        }


    }
}
