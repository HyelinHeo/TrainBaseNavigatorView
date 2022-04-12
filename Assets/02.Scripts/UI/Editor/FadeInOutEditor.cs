using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FadeInOut))]
public class FadeInOutEditor : Editor {

    public FadeInOut fadeInOut;

    public virtual void OnEnable()
    {
        fadeInOut = target as FadeInOut;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Fade In"))
        {
            //fadeInOut.StartCoroutine(fadeInOut.Fadein);
            fadeInOut.FadeIn();
        }
        if (GUILayout.Button("Fade Out"))
        {
            fadeInOut.StartCoroutine(fadeInOut.Fadeout);
            //fadeInOut.FadeOut();
        }
    }
}