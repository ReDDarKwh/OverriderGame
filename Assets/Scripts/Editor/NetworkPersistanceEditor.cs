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

        base.OnInspectorGUI();
    }
}
