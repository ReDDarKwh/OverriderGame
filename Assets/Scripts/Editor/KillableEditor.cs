using UnityEngine;
using System.Collections;
using UnityEditor;
using Scripts.Hacking;

[CustomEditor(typeof(Killable))]
public class KillableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        Killable component = (Killable)target;
        if (GUILayout.Button("KILL"))
        {
            component.InflictDamage(component.health, DamageType.Explosion);
        }
    }
}
