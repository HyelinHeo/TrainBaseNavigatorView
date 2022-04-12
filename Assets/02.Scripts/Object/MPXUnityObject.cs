using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MPXObject;
using MPXRemote.Message;
using UnityEngine.EventSystems;
using System;
using MPXObject.NaviWorkObject;

public interface ISend
{
    void EndProcess(EventCreateObject obj);
    void Send(EventCreateObject obj);
}

public class MPXUnityObject : MonoBehaviour, ISend
{
    public bool isNew;
    public Outline SelectEffect;

    public UnityEngine.Color RgbColor;
    public MPXObject.Color MpxColor;

    public Collider[] MyCol;
    //public LayerMask MyLayer;
    public Transform Mytr;

    public string ID;
    public string Name;

    public List<MPXUnityObjectChild> Children;

    public virtual void Init()
    {
        //Init(true);
    }

    public void Init(bool isnew = true)
    {
        isNew = isnew;
        Init();
    }

    private void OnDestroy()
    {
    }

    Transform SetPointsIntoTransform(Point3 pos, Point3 rot, Point3 size)
    {
        Mytr.position = CreateMPXObject.Point3ToVector3(pos);
        Mytr.eulerAngles = CreateMPXObject.Point3ToVector3(rot);
        Mytr.localScale = CreateMPXObject.Point3ToVector3(size);

        return Mytr;
    }

    public void AddSelectEffect()
    {
        SelectEffect = gameObject.AddComponent<Outline>();
        SelectEffect.enabled = false;
    }

    public Transform SetTransform(Transform tr)
    {
        Mytr = tr;
        return Mytr;
    }

    public Vector3 GetCenter()
    {
        if (MyCol != null)
        {
            return GetBounds().center;
        }
        return Vector3.zero;
    }

    public Vector3 GetCenterForward()
    {
        if (MyCol != null)
        {
            return GetBounds().center + transform.forward * GetBounds().size.z / 2;
        }
        return Vector3.zero;
    }

    void FindEdge(Bounds b, float delay = 10)
    {
        // bottom
        var p1 = new Vector3(b.min.x, b.min.y, b.min.z);//(-10.0,0,-10.0)
        var p2 = new Vector3(b.max.x, b.min.y, b.min.z);//(10.0,0,-10.0)
        var p3 = new Vector3(b.max.x, b.min.y, b.max.z);//(10.0,0,10.0)
        var p4 = new Vector3(b.min.x, b.min.y, b.max.z);//(-10.0,0,10.0)
    }

    Bounds GetBounds()
    {
        Bounds bounds = new Bounds();

        if (MyCol != null)
        {
            for (int i = 0; i < MyCol.Length; i++)
            {
                bounds.Encapsulate(MyCol[i].bounds);
            }
        }
        else
            Debug.LogError("Collider is null");

        return bounds;
    }

    public virtual void Draw(EventCreateObject obj)
    {
        SetValue(obj.ObjInfo.ID, obj.ObjInfo.Name, obj.ObjInfo.Position, obj.ObjInfo.Rotation, obj.ObjInfo.Size);
    }

    public virtual void Draw()
    {
        //유니티 자체 생성
    }

    /// <summary>
    /// 유니티 자체 생성 끝났을 때
    /// </summary>
    public virtual void DrawDone(MpxNaviWorkObject obj)
    {
        EventCreateObject eCreate = new EventCreateObject();
        eCreate.Command = EventCreateObject.COMMAND_CREATE;
        eCreate.ObjectType = (int)MpxNaviWorkObject.ObjectType.FOUNDATION_OBJECT;
        eCreate.ObjInfo = obj;
        Send(eCreate);
    }

    public virtual void Modify(EventCreateObject obj)
    {
        SetValue(obj.ObjInfo.Name, obj.ObjInfo.Position, obj.ObjInfo.Rotation, obj.ObjInfo.Size);
        //if (SelectEffect != null)
        //{
        //    SelectEffect.Refresh();
        //}
    }

    public virtual void Modify()
    {

    }

    public virtual void ChangeSettings()
    {

    }

    public void SetValue(string id, string name, Point3 pos, Point3 rot, Point3 size)
    {
        ID = id;
        SetValue(name, pos, rot, size);
    }

    void SetValue(string name, Point3 pos, Point3 rot, Point3 size)
    {
        Name = name;
        Mytr = SetPointsIntoTransform(pos, rot, size);
        this.name = name;
    }

    public virtual void Delete(EventCreateObject obj)
    {
        DestroyThisObject();
    }

    void DestroyThisObject()
    {
        MPXObjectManager.Inst.RemoveObjectToDic(ID);
        Destroy(this.gameObject);
        Debug.Log("Delete Object." + Name);
    }

    /// <summary>
    /// 유니티에서 자체 편집한 값 전송 시
    /// send value which edited by Unity
    /// </summary>
    /// <param name="eCreate"></param>
    public virtual void Send(EventCreateObject eCreate)
    {
        eCreate.ObjInfo.Position = CreateMPXObject.Vector3ToPoint3(Mytr.position);
        eCreate.ObjInfo.Rotation = CreateMPXObject.Vector3ToPoint3(Mytr.eulerAngles);
        eCreate.ObjInfo.Size = CreateMPXObject.Vector3ToPoint3(Mytr.localScale);
        SenderManager.Inst.SendRequest(eCreate);
    }

    public UnityEngine.Color MpxColorToUnityColor(MPXObject.Color color)
    {
        byte r = byte.Parse(color.Red.ToString());
        byte g = byte.Parse(color.Green.ToString());
        byte b = byte.Parse(color.Blue.ToString());
        byte a = byte.Parse(color.Alpha.ToString());
        RgbColor = new Color32(r, g, b, a);
        return RgbColor;
    }

    public MPXObject.Color UnityColorToMpxColor(UnityEngine.Color color)
    {
        MpxColor = new MPXObject.Color((int)color.r, (int)color.g, (int)color.b, (int)color.a);
        return MpxColor;
    }

    public void ByteToTexture(byte[] imageData, Material mat)
    {
        Texture2D tx = new Texture2D(1, 1);
        tx.LoadImage(imageData);

        tx.hideFlags = HideFlags.HideAndDontSave;
        tx.filterMode = FilterMode.Point;
        tx.Apply();

        SetTextureToMaterial(tx, mat);
    }

    public void SetTextureToMaterial(Texture tx, Material mat)
    {
        mat.SetTexture("_MainTex", tx);
    }

    public virtual MpxObject TransMpxObject()
    {
        return null;
    }

    /// <summary>
    /// 윈폼에서 요청한 값 전송 시
    /// send value which requested by Winform
    /// </summary>
    public virtual void EndProcess(EventCreateObject eCreate)
    {
        SenderManager.Inst.EndProcess(eCreate.ID);
    }

    public void ChangePivot(ObjPivot pivot)
    {
        Bounds bounds = new Bounds();
        for (int i = 0; i < Children.Count; i++)
        {
            Children[i].transform.SetParent(null);
        }
        bounds = GetBounds();
        Vector3 pos = Vector3.zero + bounds.center;
        if (pivot == ObjPivot.Bottom)
        {
            pos.y -= bounds.size.y * 0.5f;
        }
        else if (pivot == ObjPivot.Top)
        {
            pos.y += bounds.size.y * 0.5f;
        }
        this.transform.position = pos;

        for (int i = 0; i < Children.Count; i++)
        {
            Children[i].transform.SetParent(this.transform);
        }
    }

    //public virtual void SetLayer()
    //{
    //    Utility.SetLayer(gameObject, MyLayer);
    //}

    //public void SetIgnoreLayer()
    //{
    //    Utility.SetLayerIgnore(gameObject);
    //}
}
