using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChangeLight))]
public class ChangeLightEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ChangeLight light = (ChangeLight)target;

        if (GUILayout.Button("Change Light Color"))
        {
            light.ChangeLightColor();
        }
        if (GUILayout.Button("Change Light Intensity"))
        {
            light.LightIntensity();
        }
    }
}
