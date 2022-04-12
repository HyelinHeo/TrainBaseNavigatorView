using MPXObject;
using MPXObject.NaviWorkObject;
using MPXRemote.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MegaShape))]
public class MPXRail : MPXUnityObject
{
    public MpxFoundationObjectRail MyClass;
    public MegaShape shape;

    public Transform Point1;
    public Transform Point2;
    public Transform Point3;
    public Transform Point4;
    public LineRenderer LineRen1;
    public LineRenderer LineRen2;

    /// <summary>
    /// invec~outvec
    /// </summary>
    [SerializeField]
    float percent = 0.3f;
    public Vector3 StartPos;
    public Vector3 EndPos;
    public Vector3 Size;
    //public Vector3 pos1Invec;
    //public Vector3 pos1Outvec;
    //public Vector3 pos2Invec;
    //public Vector3 pos2Outvec;
    public float Width;
    public float Height;

    public MeshCollider ColMesh;
    public Material MyMat;

    private void Start()
    {
        tag = TagManager.RAIL;
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
        if (CompareTag(TagManager.RAIL))
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
        MyClass = (MpxFoundationObjectRail)obj.ObjInfo;
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
        Height = MyClass.Size.Z;
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

            SetPosition();

            SetVector();
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

    public void OnSelected()
    {
        Point1.localPosition = knot0.invec;
        Point2.localPosition = knot0.outvec;
        Point3.localPosition = knot1.invec;
        Point4.localPosition = knot1.outvec;
        LineRen1.SetPosition(0, Point1.position);
        LineRen1.SetPosition(1, Point2.position);
        LineRen2.SetPosition(0, Point3.position);
        LineRen2.SetPosition(1, Point4.position);
        ShowBezierLine(true);
    }

    public void UnSelected()
    {
        ShowBezierLine(false);
    }

    void ShowBezierLine(bool show)
    {
        LineRen1.enabled = show;
        LineRen2.enabled = show;
        Point1.gameObject.SetActive(show);
        Point2.gameObject.SetActive(show);
        Point3.gameObject.SetActive(show);
        Point4.gameObject.SetActive(show);
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
        //idx.invec = pos + posInvec;
        //idx.outvec = pos + posOutvec;
    }

    public override void Draw(EventCreateObject obj)
    {
        MyClass = (MpxFoundationObjectRail)obj.ObjInfo;
        SetValue(MyClass.ID, MyClass.Name, MyClass.Position, MyClass.Rotation, MPXObject.Point3.One);
        StartPos = CreateMPXObject.Point3ToVector3(MyClass.StartPos);
        EndPos = CreateMPXObject.Point3ToVector3(MyClass.EndPos);
        Size = CreateMPXObject.Point3ToVector3(MyClass.Size);
        
        Width = Size.x;
        Height = Size.z;
        if (shape != null)
        {
            Draw();
            SetOutvec(knot0, CreateMPXObject.Point3ToVector3(MyClass.Curve1Pos));
            SetOutvec(knot1, CreateMPXObject.Point3ToVector3(MyClass.Curve2Pos));
            ReCalculateMesh();
            SelectEffect.RefreshRail();

            MpxColor = MyClass.BaseColor;
            MyMat = GetComponent<Renderer>().material;
            MyMat.color = MpxColorToUnityColor(MpxColor);
            if (MyClass.Texture != null)
            {
                ByteToTexture(MyClass.Texture.Bytes, MyMat);
                MyMat.mainTextureScale = Vector2.one * Size.z;
            }
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
            MyClass = (MpxFoundationObjectRail)obj;
        else
            MyClass = new MpxFoundationObjectRail();
        MyClass = (MpxFoundationObjectRail)TransMpxObject();
        MyClass.BaseColor = UnityColorToMpxColor(MyMat.color);
        SelectEffect.RefreshRail();
        base.DrawDone(MyClass);
    }

    void SetVector()
    {
        knot0.outvec = Vector3.Lerp(knot0.p, knot1.p, percent);
        SetInvec(knot0, knot0.outvec);
        knot1.invec = Vector3.Lerp(knot1.p, knot0.p, percent);
        SetOutvec(knot1, knot1.invec);
    }

    void ReCalculateMesh()
    {
        shape.ReCalcLength();
        shape.ResetMesh();
        ColMesh.sharedMesh = GetComponent<MeshFilter>().sharedMesh;
        ColMesh.convex = false;
        MyCol[0] = ColMesh;
    }

    void SetInvec(MegaKnot knot, Vector3 outvec)
    {
        knot.outvec = outvec;
        Vector3 distance = knot.p - outvec;
        knot.invec = distance + knot.p;
    }

    void SetOutvec(MegaKnot knot, Vector3 invec)
    {
        knot.invec = invec;
        Vector3 distance = knot.p - invec;
        knot.outvec = distance + knot.p;
    }

    public override void Modify()
    {
        SelectEffect.RefreshRail();
        ColMesh.sharedMesh = GetComponent<MeshFilter>().sharedMesh;
        ColMesh.convex = false;
        MyCol[0] = ColMesh;
        //test
        base.Modify();
        EventCreateObject eCreate = new EventCreateObject();
        eCreate.Command = EventCreateObject.COMMAND_EDIT;
        eCreate.ObjectType = (int)MpxNaviWorkObject.ObjectType.FOUNDATION_OBJECT;
        GetComponent<Renderer>().material = MyMat;
        eCreate.ObjInfo = TransMpxObject() as MpxFoundationObjectRail;
        Send(eCreate);
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
        MyClass.Curve1Pos = CreateMPXObject.Vector3ToPoint3(knot0.invec);
        MyClass.Curve2Pos = CreateMPXObject.Vector3ToPoint3(knot1.invec);
        if (MyMat != null)
        {
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
        //ChangeValue(eCreate);
        SenderManager.Inst.SendRequest(eCreate);
    }

    void ChangeValue(EventCreateObject eCreate)
    {
        eCreate.ObjInfo.Position = CreateMPXObject.Vector3ToPoint3(Mytr.position);
        eCreate.ObjInfo.Rotation = CreateMPXObject.Vector3ToPoint3(Mytr.eulerAngles);
        eCreate.ObjInfo.Size = CreateMPXObject.Vector3ToPoint3(Size);
    }
}
