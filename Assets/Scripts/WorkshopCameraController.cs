using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkshopCameraController : MonoBehaviour
{
    public Transform target;

    public float distance = 5.0f;
    public float maxDistance = 20;
    public float minDistance = .6f;
    public Camera cam;

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;
    private Vector3 lastPosition;

    void Start() { Init(); }
    void OnEnable() { Init(); }

    public void Init()
    {
        //If there is no target, create a temporary target at 'distance' from the cameras current viewpoint
        if (!target)
        {
            GameObject go = new GameObject("Cam Target");
            go.transform.position = transform.position + (transform.forward * distance);
            target = go.transform;
        }

        distance = Vector3.Distance(transform.position, target.position);
        currentDistance = distance;
        desiredDistance = distance;

        //be sure to grab the current rotations as starting points.
        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;

        xDeg = Vector3.Angle(Vector3.right, transform.right);
        yDeg = Vector3.Angle(Vector3.up, transform.up);
    }

    void ZoomOrthoCamera(Vector3 zoomTowards, float amount)
    {
        if (this.cam.orthographicSize - amount < maxDistance && this.cam.orthographicSize - amount > minDistance)
        {
            float multiplier = (1.0f / this.cam.orthographicSize * amount);
            transform.position += (zoomTowards - transform.position) * multiplier;
            this.cam.orthographicSize -= amount;
        }
    }

    void LateUpdate()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            ZoomOrthoCamera(cam.ScreenToWorldPoint(Input.mousePosition), 1);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            ZoomOrthoCamera(cam.ScreenToWorldPoint(Input.mousePosition), -1);
        }

        if (Input.GetMouseButtonDown(2))
        {
            lastPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(2))
        {
            var difference = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            transform.position = lastPosition - difference;
        }
    }
}