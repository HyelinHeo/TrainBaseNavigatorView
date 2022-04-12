using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MPXRail))]
public class MPXRailEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MPXRail rail = target as MPXRail;
        if (GUILayout.Button("OnSelect"))
        {
            rail.OnSelected();
        }
        if (GUILayout.Button("UnSelect"))
        {
            rail.UnSelected();
        }
    }
}
