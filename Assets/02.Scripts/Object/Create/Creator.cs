using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creator : MonoBehaviour
{
    public bool UseCreator;
    public MPXUnityObject Prefab;

    protected MPXUnityObject CurrentObject;

    protected virtual void Start()
    {
        Init();
    }
    /// <summary>
    /// 새작업시 실행
    /// </summary>
    public virtual void InitNew() { }

    /// <summary>
    /// 불러오기 실행
    /// </summary>
    public virtual void InitLoad() { }

    public virtual MPXUnityObject Create()
    {
        MPXUnityObject obj = CreatePreview();
        if (obj != null)
        {
            obj.Init();
        }
        //obj.SetLayer();

        return obj;
    }

    public virtual MPXUnityObject CreatePreview()
    {
        if (Prefab != null)
        {
            MPXUnityObject go = Instantiate(Prefab);
            //go.PrefabName = Prefab.name;
            go.transform.SetParent(transform);
            //go.SetIgnoreLayer();

            return go;
        }
        else
        {
            Debug.LogWarning("prefab not found.");
            return null;
        }
    }

    public virtual void Init()
    {
        CurrentObject = null;
    }

    public virtual void DestoryObject()
    {
        if (CurrentObject != null)
        {
            Destroy(CurrentObject.gameObject);
        }
        CurrentObject = null;
    }

    public void CreateObject() { Create(); }

}
