using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLight : MonoBehaviour
{
    public Light[] DirectionalLight;
    //hexColor 값
    public string HexColor;

    //rgbColor 값
    public Color RgbColor;
    Color color;

    [Range(0, 2)]
    public float Intensity;

    public void ChangeLightColor()
    {
        for (int i = 0; i < DirectionalLight.Length; i++)
        {
            ColorUtility.TryParseHtmlString(HexColor, out color);
            DirectionalLight[i].color = color;

            //DirectionalLight[i].color = RgbColor;//new Color(0,10,0);
        }
    }

    public void LightIntensity()
    {
        for (int i = 0; i < DirectionalLight.Length; i++)
        {
            DirectionalLight[i].intensity = Intensity;
        }
    }
}
