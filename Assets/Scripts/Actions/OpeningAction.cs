using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OpeningAction : Action
{
    public Vector3 openPositionOffset;
    public float movingSpeed;
    public Transform door;
    public Collider2D doorCollider;
    private Vector3 doorStartPos;
    private float pos;

    internal bool IsDoorClosing()
    {
        return !outputGate.currentValue && pos > 0;
    }

    internal override void OnStart()
    {
        doorStartPos = door.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        pos = Mathf.Clamp(pos + Time.deltaTime * movingSpeed * (outputGate.currentValue ? 1 : -1), 0, 1);
        door.transform.position = Vector3.Lerp(doorStartPos,
            doorStartPos + openPositionOffset,
            Mathf.SmoothStep(0.0f, 1.0f, pos)
        );

        doorCollider.enabled = !outputGate.currentValue;
    }
}

