using UnityEngine;
using System.Collections;
using UnityEditor;
using Scripts.Hacking;

[CustomEditor(typeof(SaveManager))]
public class SaveManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        SaveManager saveManager = (SaveManager)target;
        if (GUILayout.Button("Save level"))
        {
            
        }
    }
}
