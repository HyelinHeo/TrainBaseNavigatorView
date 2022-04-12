using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPXObject;
using MPXObject.NaviWorkObject;
using MPXRemote.Message;

public class MPXWorldPlane : MPXUnityObject
{
    public MpxWorldObject MyClass;
    public MeshRenderer MyRenderer;
    public Material FloorMat;
    public Material FloorImgMat;

    public Material[] MyMats;
    UnityEngine.Color matColor;
    //[SerializeField]
    //string ImgPath;

    public override void Init()
    {
        base.Init();
    }

    public override void Draw(EventCreateObject obj)
    {
        base.Draw(obj);
        MyClass = (MpxWorldObject)obj.ObjInfo;
        ChangeSettings();
        MPXObjectManager.Inst.SetWorldPlane(this);
        MPXObjectManager.Inst.AddObjectToList(this);
    }

    public override void Modify(EventCreateObject obj)
    {
        base.Modify(obj);
        MyClass = (MpxWorldObject)obj.ObjInfo;
        ChangeSettings();
        MPXObjectManager.Inst.SetWorldPlane(this);
    }

   public override  void ChangeSettings()
    {
        matColor = new UnityEngine.Color(1, 1, 1, MyClass.BGTransparent);
        FloorMaterialVisible(MyClass.ShowBG);
        if (MyClass.BGImage==null)
        {
            RemoveTexture();
        }
        else
        {
            ByteToTexture(MyClass.BGImage.Bytes, FloorImgMat);
        }
        FloorImgMat.mainTextureOffset = CreateMPXObject.Point2ToVector2(MyClass.Offset);
        FloorImgMat.mainTextureScale = CreateMPXObject.Point2ToVector2(MyClass.Tiling);
    }

    public override MpxObject TransMpxObject()
    {
        MyClass.Position = CreateMPXObject.Vector3ToPoint3(Mytr.position);
        MyClass.Rotation = CreateMPXObject.Vector3ToPoint3(Mytr.eulerAngles);
        MyClass.Size = CreateMPXObject.Vector3ToPoint3(Mytr.localScale);
        return MyClass;
    }

    public override void Send(EventCreateObject eCreate)
    {
        //eCreate.PType = Protocol.ProtocolType.EventCreateObject;
        //eCreate.Command = CreateMPXObject.CREATE;
        //eCreate.State = false;
        //eCreate.ObjectType = (int)MpxNaviWorkObject.ObjectType.WORLD_OBJECT;
        //MpxWorldObject wObjInfo = new MpxWorldObject();
        //wObjInfo.ID = ID;
        //wObjInfo.Name = name;
        //wObjInfo.Position = CreateMPXObject.Vector3ToPoint3(Mytr.position);
        //wObjInfo.Rotation = CreateMPXObject.Vector3ToPoint3(Mytr.eulerAngles);
        //wObjInfo.Size = CreateMPXObject.Vector3ToPoint3(Mytr.localScale);
        //eCreate.ObjInfo = wObjInfo;

        ////SenderManager.Inst.Send(eCreate);
        //base.Send(eCreate);
        //Debug.Log(wObjInfo.ToString());
    }

    public override void Delete(EventCreateObject obj)
    {
        MyClass = (MpxWorldObject)obj.ObjInfo;
        MyClass.Removed = true;
        base.Delete(obj);

        obj.ObjInfo = MyClass;
        EndProcess(obj);
    }

    void RemoveTexture()
    {
        FloorImgMat.mainTexture = null;
    }

    public void FloorMaterialVisible(bool show)
    {
        if (show)
        {
            FloorImgMat.color = matColor;
            MyRenderer.materials = new Material[2];
            MyRenderer.materials = MyMats;
        }
        else
        {
            //FloorMat.color = matColor;
            MyRenderer.materials = new Material[1];
            MyRenderer.material = FloorMat;
        }
    }

    private void OnDestroy()
    {
        MPXObjectManager.Inst.SetWorldPlane(null);
        RemoveTexture();
    }
}
