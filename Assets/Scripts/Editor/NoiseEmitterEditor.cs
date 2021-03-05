

using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(NoiseEmitter))]
public class NoiseEmitterEditor : Editor
{
    private NoiseEmitter c;

    public void OnSceneGUI()
    {
        c = this.target as NoiseEmitter;
        if (c == null)
        {
            return;
        }

        Handles.color = Color.red;

        Handles.DrawWireArc(c.transform.position, c.transform.forward,
          (Quaternion.Euler(0, 0, -360 / 2) * c.transform.right) * c.noiseRadius,
          360, c.noiseRadius);
    }
}