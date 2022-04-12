using MPXObject;
using MPXObject.NaviObject;
using MPXObject.NaviWorkObject;
using MPXRemote.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MPXSimulationImport : MPXUnityObject
{
    public MpxNaviObjectImport MyClass;
    public MpxSimulationObject ParentClass;
    public List<MPXProperty> Properties;

    public List<Material> MatList;
    public List<UnityEngine.Color> MatColorList;

    public override void Init()
    {
        Mytr = this.transform;
        MyCol = GetComponentsInChildren<Collider>();
        ChangePivot(ObjPivot.Bottom);
        base.Init();
    }

    public override void Draw(EventCreateObject obj)
    {
        base.Draw(obj);
        ParentClass = (MpxSimulationObject)obj.ObjInfo;
        ChangeSetting(ParentClass);
        EndProcess(obj);
    }

    private void ChangeSetting(MpxSimulationObject sObj)
    {
        MyClass = (MpxNaviObjectImport)sObj.MyObject;
        Properties = sObj.Properties;
        
        MpxColor = MyClass.MyColor;
        MpxColorToUnityColor(MpxColor);
        AddColorAtObj();
    }

    public override void Modify(EventCreateObject obj)
    {
        base.Modify(obj);
        ParentClass = (MpxSimulationObject)obj.ObjInfo;
        ChangeSetting(ParentClass);
        //EndProcess(obj);
    }

    /// <summary>
    /// 윈폼에서 요청 삭제시
    /// </summary>
    /// <param name="obj"></param>
    public override void Delete(EventCreateObject obj)
    {
        ParentClass = (MpxSimulationObject)obj.ObjInfo;
        ParentClass.Removed = true;
        base.Delete(obj);

        obj.ObjInfo = ParentClass;
        EndProcess(obj);
    }

    public override void EndProcess(EventCreateObject eCreate)
    {
        base.EndProcess(eCreate);
    }

    public override MpxObject TransMpxObject()
    {
        ParentClass.Position = CreateMPXObject.Vector3ToPoint3(Mytr.position);
        ParentClass.Rotation = CreateMPXObject.Vector3ToPoint3(Mytr.eulerAngles);
        ParentClass.Size = CreateMPXObject.Vector3ToPoint3(Mytr.localScale);
        ParentClass.MyObject = MyClass;
        return ParentClass;
    }

    public void AddColorAtObj()
    {
        if (MatColorList != null)
        {
            for (int i = 0; i < MatColorList.Count; i++)
            {
                MatList[i].color = (MatColorList[i] + RgbColor) * 0.5f;
            }
        }
    }
}
