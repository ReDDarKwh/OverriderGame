using UnityEngine;
using System.Collections;
using UnityEditor;
using Scripts.Hacking;

[CustomEditor(typeof(LevelEditorSaver))]
public class SaveManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        LevelEditorSaver saveManager = (LevelEditorSaver)target;
        if (GUILayout.Button("Save level"))
        {
            saveManager.SaveLevel();
        }
    }
}
