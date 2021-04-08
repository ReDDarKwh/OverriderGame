

using UnityEditor;
using UnityEngine;
using System.Collections;
using Scripts.Actions;

[CustomEditor(typeof(SpottingAction))]
public class SpottingActionEditor : Editor
{
    private SpottingAction c;

    public void OnSceneGUI()
    {
        c = this.target as SpottingAction;

        Handles.color = Color.red;

        Handles.DrawWireArc(c.transform.position, c.transform.forward,
          (Quaternion.Euler(0, 0, -c.VisionAngle / 2) * c.transform.right) * c.VisionRadius,
          c.VisionAngle, c.VisionRadius);

        Handles.DrawLine(c.transform.position, c.transform.position + (Quaternion.Euler(0, 0, -c.VisionAngle / 2) * (c.transform.rotation * Vector3.right)) * c.VisionRadius);
        Handles.DrawLine(c.transform.position, c.transform.position + (Quaternion.Euler(0, 0, c.VisionAngle / 2) * (c.transform.rotation * Vector3.right)) * c.VisionRadius);
        // radius
    }
}