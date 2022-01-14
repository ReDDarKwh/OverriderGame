using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[ExecuteInEditMode]
 public class EditorAutoPlacer : MonoBehaviour {
    public string parentTag;
    private bool alreadyPlaced;
    void Awake()
    {
        if (Application.isEditor && !alreadyPlaced && PrefabStageUtility.GetCurrentPrefabStage() == null)
        {
            transform.parent = GameObject.FindWithTag(parentTag).transform;
            alreadyPlaced = true;
        }
    }
 }