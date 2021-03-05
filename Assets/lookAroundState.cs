using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lookAroundState : MonoBehaviour
{
    public Creature creature;
    public float speed;
    public float relativeMinAngle;
    public float relativeMaxAngle;
    private Quaternion startRotation;
    private Quaternion minRotation;
    private Quaternion maxRotation;
    private Quaternion targetRotation;
    private float time;
    public void StateEnter()
    {
        startRotation = Quaternion.LookRotation(Vector3.forward, creature.headDir) * Quaternion.Euler(0, 0, 90);
    }

    public void StateUpdate()
    {
        minRotation = startRotation * Quaternion.Euler(0, 0, -relativeMinAngle);
        maxRotation = startRotation * Quaternion.Euler(0, 0, relativeMaxAngle);

        time += Time.deltaTime;
        creature.headDir = Quaternion.Lerp(minRotation, maxRotation, (Mathf.Sin(time * speed) + 1) / 2) * Vector3.right;
    }

}
