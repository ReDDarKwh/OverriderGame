using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public Rigidbody2D rb;
    public Rigidbody2D target;
    public float SmoothTime;
    private Vector3 velocity;

    public float maxDistance = 20;
    public float minDistance = .6f;


    // Start is called before the first frame update
    void Start()
    {

    }

    void ZoomOrthoCamera(Vector3 zoomTowards, float amount)
    {
        if (Camera.main.orthographicSize - amount < maxDistance && Camera.main.orthographicSize - amount > minDistance)
        {
            float multiplier = (1.0f / Camera.main.orthographicSize * amount);
            transform.position += (zoomTowards - transform.position) * multiplier;
            Camera.main.orthographicSize -= amount;
        }
    }

    void LateUpdate()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetPosition = Vector3.Lerp(target.position, mousePos, 0.25f);

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            ZoomOrthoCamera(targetPosition, 1);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            ZoomOrthoCamera(targetPosition, -1);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetPosition = Vector3.Lerp(target.position, mousePos, 0.25f);
        rb.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, SmoothTime);
    }
}
