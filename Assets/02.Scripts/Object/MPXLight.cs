using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPXObject;
using MPXObject.NaviWorkObject;
using MPXRemote.Message;

public class MPXLight : MPXUnityObject
{
    public MpxLight MyClass;
    public Light myLight;
    //hexColor 값
    //string HexColor;

    [Range(0, 10)]
    public float Intensity;

    public override void Init()
    {
        base.Init();
        RgbColor = myLight.color;
        Intensity = myLight.intensity;
    }

    public override void Draw(EventCreateObject obj)
    {
        base.Draw(obj);
        MyClass = (MpxLight)obj.ObjInfo;
        Mytr.position = new Vector3(0, 3, 5);
        Mytr.eulerAngles = new Vector3(50, -30, 0);
        Mytr.localScale = Vector3.one;
        ChangeSettings();
    }

    public override void EndProcess(EventCreateObject eCreate)
    {
        eCreate.ObjInfo = TransMpxObject() as MpxLight;
        base.EndProcess(eCreate);
    }

    public override MpxObject TransMpxObject()
    {
        MyClass.Position = CreateMPXObject.Vector3ToPoint3(Mytr.position);
        MyClass.Rotation = CreateMPXObject.Vector3ToPoint3(Mytr.eulerAngles);
        MyClass.Size = CreateMPXObject.Vector3ToPoint3(Mytr.localScale);
        return MyClass;
    }

    public override void Modify(EventCreateObject obj)
    {
        base.Modify(obj);
        MyClass = obj.ObjInfo as MpxLight;
        ChangeSettings();
    }

    public override void Delete(EventCreateObject obj)
    {
        MyClass = (MpxLight)obj.ObjInfo;
        MyClass.Removed = true;
        base.Delete(obj);

        obj.ObjInfo = MyClass;
        EndProcess(obj);
    }

    public override void ChangeSettings()
    {
        MpxColor = MyClass.Color;
        myLight.color = MpxColorToUnityColor(MpxColor);

        Intensity = MyClass.Brightness;
        myLight.intensity = Intensity;
    }

    public override void Send(EventCreateObject eCreate)
    {
        //EventCreateObject eCreate = new EventCreateObject();
        //eCreate.PType = Protocol.ProtocolType.EventCreateObject;
        //eCreate.Command = command;
        //eCreate.State = false;
        //eCreate.ObjectType = (int)MpxNaviWorkObject.ObjectType.WORLD_OBJECT;
        //WorldObjectInfo wObjInfo = new WorldObjectInfo();
        //wObjInfo.ID = ID;
        //wObjInfo.Name = name;
        //wObjInfo.Position = CreateMPXObject.Vector3ToPoint3(Mytr.position);
        //wObjInfo.Rotation = CreateMPXObject.Vector3ToPoint3(Mytr.eulerAngles);
        //wObjInfo.Size = CreateMPXObject.Vector3ToPoint3(Mytr.localScale);
        //eCreate.ObjInfo = wObjInfo;

        //SenderManager.Inst.Send(eCreate);
        //Debug.Log(wObjInfo.ToString());

        base.Send(eCreate);
    }
    
    public UnityEngine.Color ChangeLightColor(MPXObject.Color color)
    {
        byte r = byte.Parse(color.Red.ToString());
        byte g = byte.Parse(color.Green.ToString());
        byte b = byte.Parse(color.Blue.ToString());
        byte a = byte.Parse(color.Alpha.ToString());
        RgbColor = new Color32(r,g,b,a);
        return RgbColor;
    }

    public void LightIntensity()
    {
        myLight.intensity = Intensity;
    }
}
