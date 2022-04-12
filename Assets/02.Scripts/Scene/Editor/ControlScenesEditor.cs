using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ControlScenes))]
public class ControlScenesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ControlScenes scenes = (ControlScenes)target;
        if (GUILayout.Button("Add WorkView"))
        {
            scenes.StartCoroutine(scenes.AddWorkView());
        }
    }
}
