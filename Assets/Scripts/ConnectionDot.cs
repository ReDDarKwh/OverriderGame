using System.Collections;
using System.Collections.Generic;
using Scripts.Hacking;
using UnityEngine;
using Vectrosity;

public class ConnectionDot : MonoBehaviour
{
    public float dotSpeed;
    public SpriteRenderer spriteRenderer;
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
            spriteRenderer.enabled = false;
        }
        else
        {
            progress += dotSpeed * Time.unscaledDeltaTime;

            var index = Mathf.RoundToInt(Mathf.Lerp(0, line.points3.Count - 1, progress));
            if (index > -1 && index < line.points3.Count)
            {
                transform.position = line.points3[index];
            }
            else
            {
                spriteRenderer.enabled = false;
            }
        }
    }
}
