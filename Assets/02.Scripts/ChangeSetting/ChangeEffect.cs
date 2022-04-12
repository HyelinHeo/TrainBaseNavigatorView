using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPXObject;
using MPXRemote.Message;
using System;

/// <summary>
/// 효과 변경 클래스
/// </summary>
public class ChangeEffect : MonoBehaviour
{
    const int OBJECT_TYPE = 5;

    private void Start()
    {
        ReceiverManager.Inst.OnReceiveCreateObj.AddListener(OnReceive);
    }

    private void OnReceive(EventCreateObject eCreate)
    {
        if (eCreate.ObjectType==OBJECT_TYPE)
        {
            //오브젝트 클래스    지역변수=(오브젝트클래스)eCreate.ObjInfo;
            //if (지역변수.isShadow)
            //{
            //    ControlShadowVisibility(지역변수.state);
            //}
        }
    }

    void ControlShadowVisibility(bool show)
    {
        if (show)
            ShadowOn();
        else
            ShadowOff();
    }

    public void ShadowOn()
    {
        QualitySettings.shadows = ShadowQuality.All;
    }

    public void ShadowOff()
    {
        QualitySettings.shadows = ShadowQuality.Disable;
    }

    void ControlLightShadeVisibility(bool show)
    {
        if (show)
            OcclusionOn();
        else
            OcclusionOff();
    }

    private void OcclusionOn()
    {
        throw new NotImplementedException();
    }

    private void OcclusionOff()
    {
        throw new NotImplementedException();
    }

    private void OnDestroy()
    {
        ReceiverManager.Inst.OnReceiveCreateObj.RemoveListener(OnReceive);
    }
}
