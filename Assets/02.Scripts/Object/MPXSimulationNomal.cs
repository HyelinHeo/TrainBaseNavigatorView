using System;
using System.Collections;
using System.Collections.Generic;
using MPXObject;
using MPXObject.NaviObject;
using MPXObject.NaviWorkObject;
using MPXRemote.Message;
using UnityEngine;

public class MPXSimulationNomal : MPXUnityObject
{
    public MeshRenderer MyRenderer;
    public MpxNaviObjectNormal MyClass;
    MpxSimulationObject ParentClass;

    public List<MPXProperty> Properties;
    public MpxNaviObjectNormal.PrimitiveType myType;

    [SerializeField]
    Material myMaterial;

    public override void Init()
    {
        base.Init();
        myMaterial = new Material(MyRenderer.material);
    }

    public override void Draw(EventCreateObject obj)
    {
        base.Draw(obj);
        ParentClass = (MpxSimulationObject)obj.ObjInfo;
        ChangeSetting(ParentClass);
        EndProcess(obj);
    }

    void ChangeSetting(MpxSimulationObject sObj)
    {
        MyClass = (MpxNaviObjectNormal)sObj.MyObject;
        Properties = sObj.Properties;

        myType = MyClass.PrimitiType;

        MpxColor = MyClass.MyColor;
        myMaterial.color = MpxColorToUnityColor(MpxColor);
        MyRenderer.material = myMaterial;
    }

    public override MpxObject TransMpxObject()
    {
        ParentClass.Position = CreateMPXObject.Vector3ToPoint3(Mytr.position);
        ParentClass.Rotation = CreateMPXObject.Vector3ToPoint3(Mytr.eulerAngles);
        ParentClass.Size = CreateMPXObject.Vector3ToPoint3(Mytr.localScale);
        ParentClass.MyObject = MyClass;
        return ParentClass;
    }

    public override void Modify(EventCreateObject obj)
    {
        base.Modify(obj);
        ParentClass = (MpxSimulationObject)obj.ObjInfo;
        ChangeSetting(ParentClass);

        //EndProcess(obj);
    }

    public override void Delete(EventCreateObject obj)
    {
        ParentClass = (MpxSimulationObject)obj.ObjInfo;
        ParentClass.Removed = true;
        base.Delete(obj);

        obj.ObjInfo = ParentClass;
        EndProcess(obj);
    }

    public override void Send(EventCreateObject eCreate)
    {
        base.Send(eCreate);
    }

    public override void EndProcess(EventCreateObject eCreate)
    {
        base.EndProcess(eCreate);
    }

    public void AddCollider()
    {

    }
}
