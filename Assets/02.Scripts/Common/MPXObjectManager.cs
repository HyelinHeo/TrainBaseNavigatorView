using MPXObject;
using MPXRemote.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ObjPivot
{
    Bottom = 0,
    Center = 1,
    Top = 2
}

public class MPXObjectManager : Singleton<MPXObjectManager>
{
    public Dictionary<string, MPXUnityObject> ObjectsDic = new Dictionary<string, MPXUnityObject>();

    public List<MPXUnityObject> SelectObjects = new List<MPXUnityObject>();
    public int SelectObjectCount { get { if (SelectObjects != null) return SelectObjects.Count; else return 0; } }
    MPXWorldPlane worldPlane;

    /// <summary>
    /// Create base Object in unity
    /// </summary>
    public class OnChangeSelectObject : UnityEvent<MPXUnityObject> { }
    public OnChangeSelectObject ChangeSelect = new OnChangeSelectObject();

    public UnityEvent SetDefaultSelectObject = new UnityEvent();

    public UnityEvent SetWorld = new UnityEvent();

    //public IEnumerator  RemoveObjectAll()
    //{
    //    while (CurrentObjects.Count>0)
    //    {

    //    }
    //    yield return null;
    //}

    public override void Init()
    {
        base.Init();
        SetDefaultSelectObject.AddListener(SetDefaultSelect);
    }

    /// <summary>
    /// 오브젝트와 UI를 제외한 모든 곳 클릭시 SelectObject는 WorldPlane이 되도록
    /// </summary>
    private void SetDefaultSelect()
    {
        if (worldPlane != null)
        {
            if (FindMPXObject().ID != worldPlane.ID)
            {
                AddObjectToList(worldPlane);
            }
        }
    }

    public void ReceiveUnSelect()
    {
        if (worldPlane != null)
        {
            if (FindMPXObject().ID != worldPlane.ID)
            {
                RemoveAllList();
                if (SelectObjects == null)
                    SelectObjects = new List<MPXUnityObject>();
                SelectObjects.Add(worldPlane);
                ChangeSelect.Invoke(worldPlane);
            }
        }
    }

    /// <summary>
    /// 월드가 존재하면 true, 존재하지 않으면 false 반환
    /// </summary>
    /// <returns></returns>
    public bool IsExistWorld()
    {
        return worldPlane == null ? false : true;
    }

    void OnDestroy()
    {
        Clear();
        ObjectsDic = null;
        SelectObjects = null;
    }

    public void Clear()
    {
        if (ObjectsDic != null)
        {
            ObjectsDic.Clear();
        }
        if (SelectObjects != null)
        {
            SelectObjects.Clear();
        }
    }

    /// <summary>
    /// select object list
    /// </summary>
    public void AddObjectToList(MPXUnityObject obj)
    {
        RemoveAllList();
        if (SelectObjects != null)
        {
            //Debug.LogFormat("Name : {0}, Size : {1}", obj.Name, obj.Mytr.localScale);
            SelectObjects.Add(obj);
            ChangeSelect.Invoke(obj);
            if (obj.ID.Equals(worldPlane.ID))
                SenderManager.Inst.SendUnSelectObject();
        }
        else
            Debug.LogError("Select Object List value is null");
    }

    /// <summary>
    /// remove object list
    /// </summary>
    public void RemoveObjectToList(MPXUnityObject obj)
    {
        if (SelectObjects != null)
        {
            if (SelectObjects.Contains(obj))
            {
                SelectObjects.Remove(obj);
            }
            else
                Debug.LogError(obj.name + " does not exist in List");
        }
        else
            Debug.LogError("Select Object List value is null");
    }

    public MPXUnityObject FindMPXObject()
    {
        if (SelectObjects != null && SelectObjects.Count > 0)
        {
            return SelectObjects[0];
        }
        else
        {
            //Debug.LogFormat("Select List is {0}, Select List count is {1}, check SelectObject.",SelectObjects,SelectObjects.Count);
            return null;
        }
    }


    public void RemoveAllList()
    {
        if (SelectObjects != null)
        {
            SelectObjects.Clear();
        }
        else
            Debug.LogError("Select Object List value is null");
    }

    public void AddObjectToDic(string code, MPXUnityObject obj)
    {
        if (!ObjectsDic.ContainsKey(code))
            ObjectsDic.Add(code, obj);
        else
            Debug.LogError("the id is already exists.");

    }

    public void RemoveObjectToDic(string code)
    {
        if (ObjectsDic.ContainsKey(code))
            ObjectsDic.Remove(code);
        else
            Debug.LogError("the id is already remove.");
    }

    public MPXUnityObject FindMPXObject(string id)
    {
        if (ObjectsDic.ContainsKey(id))
        {
            //Debug.Log(ObjectsDic[id].name + " , " + ObjectsDic[id].Name);
            return ObjectsDic[id];
        }
        //Debug.LogError("doesn't exists in Dictionary.");
        return null;
    }

    public bool ContainPartState(string id)
    {
        return ObjectsDic.ContainsKey(id);
    }

    public void SetWorldPlane(MPXWorldPlane obj)
    {
        worldPlane = obj;
        SetWorld.Invoke();
    }

    public MPXWorldPlane GetWorldPlane()
    {
        return IsExistWorld() ? worldPlane : null;
    }
}
