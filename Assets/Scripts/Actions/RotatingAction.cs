using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingAction : Action
{
    public float speed;
    public float relativeMinAngle;
    public float relativeMaxAngle;

    private Quaternion minRotation;
    private Quaternion maxRotation;
    private Quaternion targetRotation;
    private float time;

    internal override void OnStart()
    {
        minRotation = transform.rotation * Quaternion.Euler(0, 0, -relativeMinAngle);
        maxRotation = transform.rotation * Quaternion.Euler(0, 0, relativeMaxAngle);
    }

    // Update is called once per frame
    void Update()
    {
        if (outputGate.currentValue)
        {
            time += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(minRotation, maxRotation, (Mathf.Sin(time * speed) + 1) / 2);
        }
    }
}
