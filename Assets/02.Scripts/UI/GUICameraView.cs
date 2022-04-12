using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum CameraMode
{
    CAMERA2D = 0,
    CAMERA3D = 1,
}

public class GUICameraView : MonoBehaviour
{

    public Image Image2D;
    public Image Image3D;

    public Color DefaultColor;
    public Color HighLightColor;
    

    public void OnClick2D()
    {
        Image2D.color = HighLightColor;
        Image3D.color = DefaultColor;
        ControlScenes.Inst.ChangeCameraMode.Invoke(CameraMode.CAMERA2D);
    }

    public void OnClick3D()
    {
        Image2D.color = DefaultColor;
        Image3D.color = HighLightColor;
        ControlScenes.Inst.ChangeCameraMode.Invoke(CameraMode.CAMERA3D);
    }
}
