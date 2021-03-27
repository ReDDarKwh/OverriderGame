using System.Collections;
using System.Collections.Generic;
using Scripts.Hacking;
using UnityEngine;
using Vectrosity;

public class ConnectionDot : MonoBehaviour
{
    public float dotSpeed;
    internal VectorLine line;
    internal AbstractGate gate;
    internal Connection connection;
    private float progress;

    // Update is called once per frame
    void Update()
    {
        if (progress > 1 || !gate.currentValue || connection == null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            progress += dotSpeed * Time.unscaledDeltaTime;
            transform.position = line.points3[Mathf.RoundToInt(Mathf.Lerp(0, line.points3.Count - 1, progress))];
        }
    }
}
