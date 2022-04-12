using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Outline))]
public class OutlineEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Outline line = target as Outline;
        if (GUILayout.Button("Outline Refresh"))
        {
            line.Refresh();
        }
    }
}
