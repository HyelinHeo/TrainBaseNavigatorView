using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestCamera))]
public class TestCameraEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TestCamera test = (TestCamera)target;
        if (GUILayout.Button("Test 3D"))
        {
            test.Change3DCameraPosition();
        }
        if (GUILayout.Button("Test 2D"))
        {
            test.Change2DCameraPosition();
        }
    }
}
