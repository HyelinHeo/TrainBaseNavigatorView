using System.Collections;
using System.Collections.Generic;
using MPXObject;
using MPXObject.NaviWorkObject;
using MPXRemote.Message;
using UnityEngine;

[RequireComponent(typeof(MegaShape))]
public class MPXWall : MPXUnityObject
{
    public MpxFoundationObjectWall MyClass;
    public MegaShape shape;

    public Vector3 StartPos;
    public Vector3 EndPos;
    public Vector3 Size;
    public float Width;
    public float Height;

    public MeshCollider ColMesh;
    public Material MyMat;

    private void Start()
    {
        tag = TagManager.WALL;
        Init();
    }


    public override void Init()
    {
        base.Init();
    }

    void SetDefauilt()
    {
        //MyCol[0] = ColMesh;
        MyMat = GetComponent<Renderer>().material;
        ID = MyClass.GetNewID();
        Name = this.name;
        MPXObjectManager.Inst.AddObjectToDic(this.ID, this);
        //MyMat=
        base.Init();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (CompareTag(TagManager.WALL))
        {
            //print(collision.transform.name);
        }
    }

    public override void Delete(EventCreateObject obj)
    {
        base.Delete(obj);
    }

    //public override void Draw(List<MPXProperty> properties)
    //{
    //    base.Draw(properties);
    //}

    public override void Modify(EventCreateObject obj)
    {
        MyClass = (MpxFoundationObjectWall)obj.ObjInfo;
        ChangeSettings();

        ChangeValue(obj);
        SelectEffect.RefreshObject();
    }

    public override void ChangeSettings()
    {
        base.ChangeSettings();

        Name = MyClass.Name;
        this.name = Name;
        Mytr.position = CreateMPXObject.Point3ToVector3(MyClass.Position);
        Mytr.eulerAngles = CreateMPXObject.Point3ToVector3(MyClass.Rotation);

        MpxColor = MyClass.BaseColor;
        MyMat.color = MpxColorToUnityColor(MpxColor);
        if (MyClass.Texture != null)
        {
            ByteToTexture(MyClass.Texture.Bytes, MyMat);
        }
        Width = MyClass.Size.X;
        Height = MyClass.Size.Y;
        shape.boxheight = Height;
        shape.boxwidth = Width;
        if (StartPos != CreateMPXObject.Point3ToVector3(MyClass.StartPos)
            || EndPos != CreateMPXObject.Point3ToVector3(MyClass.EndPos))
        {
            StartPos = CreateMPXObject.Point3ToVector3(MyClass.StartPos);
            EndPos = CreateMPXObject.Point3ToVector3(MyClass.EndPos);
            Draw();
        }
        else if (Size.z != MyClass.Size.Z)
        {
            Size = CreateMPXObject.Point3ToVector3(MyClass.Size);
            shape.splines[0].length = Size.z;//todo
        }
        MyMat.mainTextureScale = Vector2.one;
    }

    [SerializeField]
    Vector3 pos0;
    [SerializeField]
    Vector3 pos1;
    MegaKnot knot0;
    MegaKnot knot1;
    public override void Draw()
    {
        if (shape != null && (EndPos - StartPos).sqrMagnitude > 0.1f)
        {
            transform.position = Vector3.Lerp(StartPos, EndPos, 0.5f);

            //if (!usePos)
            //{
            //    length = Size.z * 0.5f;
            //    float splineLength = shape.splines[0].length * 0.5f;
            //    float diffrence = 0;
            //    diffrence = length - splineLength;
            //    StartPos += diffrence;
            //}

            SetPosition();

            shape.boxwidth = Width;
            shape.boxheight = Height;
            shape.offset = Height / 2 * -1;

            Size.x = Width;
            Size.y = shape.splines[0].length; 
            Size.z = Height;

            shape.handleType = MegaHandleType.Free;
            shape.cap = true;
            shape.drawKnots = false;
            shape.drawHandles = false;
            shape.drawspline = false;

            shape.makeMesh = true;
            shape.meshType = MeshShapeType.Box;

            //MPXProperty p1 = GetProperty(PROPERTY_THICKNESS);
            //MPXProperty p2 = GetProperty(PROPERTY_HEIGHT);

            shape.ReCalcLength();
            shape.ResetMesh();

            if (ColMesh != null)
                Destroy(ColMesh);

            ColMesh = gameObject.AddComponent<MeshCollider>();
            ColMesh.convex = true;
            //Col.isTrigger = true;
            ColMesh.sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
            MyCol[0] = ColMesh;
        }
    }

    void SetPosition()
    {
        Vector3 pos = transform.position;

        pos0 = StartPos - pos;
        pos1 = EndPos - pos;

        knot0 = shape.splines[0].knots[0];
        knot1 = shape.splines[0].knots[1];
        SetKnots(knot0, pos0);
        SetKnots(knot1, pos1);
    }

    void SetKnots(MegaKnot idx, Vector3 pos)
    {
        idx.p = pos;
        idx.invec = pos /*+ (Vector3.left * Width * 0.05f)*/;
        idx.outvec = pos /*+ (Vector3.right * Width * 0.05f)*/;
    }

    public override void Draw(EventCreateObject obj)
    {
        MyClass = (MpxFoundationObjectWall)obj.ObjInfo;
        SetValue(MyClass.ID, MyClass.Name, MyClass.Position, MyClass.Rotation, Point3.One);
        StartPos = CreateMPXObject.Point3ToVector3(MyClass.StartPos);
        EndPos = CreateMPXObject.Point3ToVector3(MyClass.EndPos);
        Size = CreateMPXObject.Point3ToVector3(MyClass.Size);
        Width = Size.x;
        Height = Size.z;
        if (shape != null)
        {
            Draw();
            MpxColor = MyClass.BaseColor;
            MyMat = GetComponent<Renderer>().material;
            MyMat.color = MpxColorToUnityColor(MpxColor);
            if (MyClass.Texture != null)
            {
                ByteToTexture(MyClass.Texture.Bytes, MyMat);
                MyMat.mainTextureScale = Vector2.one;
            }
            SelectEffect.RefreshObject();
        }
    }

    /// <summary>
    /// 유니티 자체 생성
    /// </summary>
    /// <param name="obj">NaviWorkObject</param>
    public override void DrawDone(MpxNaviWorkObject obj)
    {
        SetDefauilt();
        if (obj != null)
            MyClass = (MpxFoundationObjectWall)obj;
        else
            MyClass = new MpxFoundationObjectWall();
        MyClass = (MpxFoundationObjectWall)TransMpxObject();
        SelectEffect.RefreshObject();
        base.DrawDone(MyClass);
    }

    public override MpxObject TransMpxObject()
    {
        MyClass.Position = CreateMPXObject.Vector3ToPoint3(Mytr.position);
        MyClass.Rotation = CreateMPXObject.Vector3ToPoint3(Mytr.eulerAngles);
        MyClass.Size = CreateMPXObject.Vector3ToPoint3(Size);
        MyClass.StartPos = CreateMPXObject.Vector3ToPoint3(StartPos);
        MyClass.EndPos = CreateMPXObject.Vector3ToPoint3(EndPos);
        MyClass.ID = ID;
        MyClass.Name = Name;
        if (MyMat != null)
        {
            //UnityColorToMpxColor(MyMat.color);
            MyClass.BaseColor = MpxColor;
        }

        return MyClass;
    }

    public override void EndProcess(EventCreateObject eCreate)
    {
        base.EndProcess(eCreate);
    }

    public override void Send(EventCreateObject eCreate)
    {
        ChangeValue(eCreate);
        SenderManager.Inst.SendRequest(eCreate);
    }

    void ChangeValue(EventCreateObject eCreate)
    {
        eCreate.ObjInfo.Position = CreateMPXObject.Vector3ToPoint3(Mytr.position);
        eCreate.ObjInfo.Rotation = CreateMPXObject.Vector3ToPoint3(Mytr.eulerAngles);
        eCreate.ObjInfo.Size = CreateMPXObject.Vector3ToPoint3(Size);
    }
}
