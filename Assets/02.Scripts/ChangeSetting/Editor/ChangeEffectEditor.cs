using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChangeEffect))]
public class ChangeEffectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ChangeEffect effect = (ChangeEffect)target;
        if (GUILayout.Button("Shadow On"))
        {
            effect.ShadowOn();
        }
        if (GUILayout.Button("Shadow Off"))
        {
            effect.ShadowOff();
        }
    }
}
