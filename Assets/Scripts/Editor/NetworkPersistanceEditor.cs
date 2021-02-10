using UnityEngine;
using System.Collections;
using UnityEditor;
using Scripts.Hacking;

[CustomEditor(typeof(NetworkPersistance))]
public class NetworkPersistanceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        NetworkPersistance np = (NetworkPersistance)target;

        if (GUILayout.Button("Save"))
        {
            np.Save();
        }

        if (GUILayout.Button("Load"))
        {
            np.Load();
        }

        base.OnInspectorGUI();
    }
}
