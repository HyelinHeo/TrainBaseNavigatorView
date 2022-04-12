using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
    /// <summary>
    /// 모든 자식 오브젝트 레이어 변경
    /// </summary>
    /// <param name="go"></param>
    /// <param name="layer"></param>
    public static void SetLayer(GameObject go, LayerMask layerMask)
    {
        go.layer = layerMask;

        Transform tr = go.transform;
        int childCount = tr.childCount;
        for (int i = 0; i < childCount; i++)
        {
            SetLayer(tr.GetChild(i).gameObject, layerMask);
        }
    }

    public static void SetLayerIgnore(GameObject go)
    {
        SetLayer(go, 2);
    }
}
