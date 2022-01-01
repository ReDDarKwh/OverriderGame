using System.Collections;
using System.Collections.Generic;
using Scripts.Hacking;
using UnityEngine;
using Vectrosity;

public class ConnectionDot : MonoBehaviour
{
    public float dotSpeed;
    public int lineDepth;
    public Material pointMaterial;
    internal VectorLine line;
    internal AbstractGate gate;
    internal Connection connection;
    private float progress;

    private VectorLine point;
    private List<Vector3> pointPointList;

    void Start(){
        point = new VectorLine("ConnectionPoint", new List<Vector3>(), 0, lineDepth);
        point.color = line.color;
        point.lineType = LineType.Points;
        point.material = pointMaterial;
        point.points3 = new List<Vector3>(){Vector3.zero};
    }

    // Update is called once per frame
    void Update()
    {

        if (progress > 1 || !gate.currentValue || connection == null)
        {
            Destroy(this.gameObject);
            VectorLine.Destroy(ref point);
        }
        else
        {

            point.SetWidth(line.GetWidth(0) + 0.25f);

            progress += dotSpeed * Time.unscaledDeltaTime;

            var index = Mathf.RoundToInt(Mathf.Lerp(0, line.points3.Count - 1, progress));
            if (index > -1 && index < line.points3.Count)
            {
                point.points3[0] = line.points3[index];
            }
            else
            {
                point.points3.Clear();
            }
        }
    }
}
