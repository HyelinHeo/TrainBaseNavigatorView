using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using MPXObject;
using MPXObject.NaviWorkObject;
using MPXRemote.Message;
using System;

public class MPXCamera : MPXUnityObject
{
    public CameraMode MyMode;

    public MpxCamera MyClass;
    public Camera MyCam;
    public CameraOperate MyCamOper;
    public PostProcessLayer ProcessLayer;
    public PostProcessVolume ProcessVolume;
    AmbientOcclusion Occlusion;


    public override void Init()
    {
        base.Init();
        ControlScenes.Inst.ChangeCameraMode.AddListener(ChangeCameraMode);
        ControlScenes.Inst.AllSceneObjectsLoded.AddListener(InitCameraPosition);
        MPXObjectManager.Inst.ChangeSelect.AddListener(ChangeSelect);
        MPXObjectManager.Inst.SetWorld.AddListener(SetWorld);
        ProcessVolume.sharedProfile.TryGetSettings(out Occlusion);
        MyCamOper.OnChangePosition.AddListener(OnChangePosition);
        if (MPXObjectManager.Inst.FindMPXObject() != null)//추후 수정_위치가 맘에안듬_20201221
            ChangeSelect(MPXObjectManager.Inst.FindMPXObject());
        isFirst = true;
    }

    private void InitCameraPosition()
    {
        CameraFullView(worldCollider);
    }

    private void SetWorld()
    {
        MyCamOper.worldObj = MPXObjectManager.Inst.GetWorldPlane();
        if (MPXObjectManager.Inst.IsExistWorld())
        {
            worldCollider = MyCamOper.worldObj.MyCol;
            MyCamOper.GetWorldBound();
            MyCamOper.operate = true;
        }
        else
        {
            MyCamOper.operate = false;
        }
    }

    private void OnCompleteCreate()
    {
        if (MyMode == CameraMode.CAMERA2D)
            ControlScenes.Inst.IsCreate2D = true;
        else if (MyMode == CameraMode.CAMERA3D)
            ControlScenes.Inst.IsCreate3D = true;

        ControlScenes.Inst.OnCompleteCreateCamera.Invoke();
    }

    [SerializeField]
    bool isFirst;
    private void ChangeSelect(MPXUnityObject obj)
    {
        MyCamOper.Target = obj;
        targetObject = obj;
        targetCollider = obj.MyCol;
    }

    public override void Draw(EventCreateObject obj)
    {
        MyClass = (MpxCamera)obj.ObjInfo;

        if (MyClass.Name.Contains("2"))
            MyMode = CameraMode.CAMERA2D;
        else
            MyMode = CameraMode.CAMERA3D;

        if (MyClass.Use)
        {
            ControlScenes.Inst.CurrentCameraMode = MyMode;
            ControlScenes.Inst.MainCam = MyCam;
        }
        else
            CameraVisible(false);

        if (MyMode == CameraMode.CAMERA2D)
        {
            MyCam.orthographic = true;
            MyCam.orthographicSize = MyClass.ZoomFactor;

            ProcessLayer.enabled = false;
            ProcessVolume.enabled = false;
        }
        else if (MyMode == CameraMode.CAMERA3D)
        {
            MyCam.orthographic = false;
            ControlShadowVisibility(MyClass.Shadow);
            ControlLightShadeVisibility(MyClass.Occlusion);
        }

        if (isNew)
        {
            ID = MyClass.ID;
            Name = MyClass.Name;
            this.name = MyClass.Name;

            if (MyMode == CameraMode.CAMERA2D)
            {
                MyCam.orthographicSize = 10;
                Mytr.position = new Vector3(0, 500, 0);
                Mytr.eulerAngles = new Vector3(90, 0, 0);
            }
            else if (MyMode == CameraMode.CAMERA3D)
            {
                Mytr.position = new Vector3(0, 5, -5);
                Mytr.eulerAngles = new Vector3(45, 0, 0);
            }
            Mytr.localScale = Vector3.one;
        }
        else
        {
            base.Draw(obj);//추후 수정 필요
        }
        ControlScenes.Inst.IntroCamera.enabled = false;
        OnCompleteCreate();
    }

    public override void Modify(EventCreateObject obj)
    {
        //base.Modify(obj);

        MyClass = (MpxCamera)obj.ObjInfo;
        if (MyMode == CameraMode.CAMERA3D)
        {
            ControlShadowVisibility(MyClass.Shadow);
            ControlLightShadeVisibility(MyClass.Occlusion);
        }
        //else if (MyMode == CameraMode.CAMERA2D)
        //{
        //    MyCam.orthographicSize = MyClass.ZoomFactor;
        //}
        if (MyClass.FullScreen)
            CameraFullView(targetCollider);
    }

    public override void Delete(EventCreateObject obj)
    {
        MyClass = (MpxCamera)obj.ObjInfo;
        MyClass.Removed = true;
        base.Delete(obj);

        obj.ObjInfo = MyClass;
        base.EndProcess(obj);
    }

    public override void EndProcess(EventCreateObject eCreate)
    {
        eCreate.ObjInfo = (MpxCamera)TransMpxObject();

        base.EndProcess(eCreate);
    }

    public override MpxObject TransMpxObject()
    {
        MyClass.Position = CreateMPXObject.Vector3ToPoint3(Mytr.position);
        MyClass.Rotation = CreateMPXObject.Vector3ToPoint3(Mytr.eulerAngles);
        MyClass.Size = CreateMPXObject.Vector3ToPoint3(Mytr.localScale);
        if (MyMode == CameraMode.CAMERA3D)
        {
            MyClass.Shadow = ShadowVisibility();
            MyClass.Occlusion = Occlusion.active;
        }
        else
        {
            MyClass.Shadow = false;
            MyClass.Occlusion = false;
            MyClass.ZoomFactor = MyCam.orthographicSize;
        }

        return MyClass;
    }


    public Vector3 WorldToScreen(Vector3 worldPos)
    {
        if (MyCam != null)
        {
            return MyCam.WorldToScreenPoint(worldPos);
        }
        return Vector3.zero;
    }

    void ControlShadowVisibility(bool show)
    {
        if (show)
            QualitySettings.shadows = ShadowQuality.All;
        else
            QualitySettings.shadows = ShadowQuality.Disable;
    }

    //void ShadowOn()
    //{
    //    QualitySettings.shadows = ShadowQuality.All;
    //}

    //void ShadowOff()
    //{
    //    QualitySettings.shadows = ShadowQuality.Disable;
    //}

    bool ShadowVisibility()
    {
        if (QualitySettings.shadows == ShadowQuality.All)
            return true;
        else
            return false;
    }

    void ControlLightShadeVisibility(bool show)
    {
        if (show)
            Occlusion.active = true;
        else
            Occlusion.active = false;
    }

    //private void OcclusionOn()
    //{
    //    Occlusion.active = true;
    //}

    //private void OcclusionOff()
    //{
    //    Occlusion.active = false;
    //}

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.F))
    //    {
    //        CameraFullView();
    //    }
    //}

    MPXUnityObject targetObject;
    Collider[] targetCollider;//선택한 오브젝트의 콜라이더(선택한 오브젝트가 없다면 월드 플레인이 중심)
    Collider[] worldCollider;//선택한 오브젝트의 콜라이더(선택한 오브젝트가 없다면 월드 플레인이 중심)
    Bounds bounds;
    //public float elevation;
    [SerializeField]
    float cameraDistance;
    float objectSize;
    float distance;
    void CameraFullView(Collider[] colls)
    {
        //if (MyCamOper.Target.CompareTag(TagManager.WORLDPLANE))
        //    cameraDistance = 3.6f;
        //else
        cameraDistance = 3.6f;
        bounds = GetBounds(colls);
        Vector3 objectSizes = bounds.max - bounds.min;
        if (MyMode == CameraMode.CAMERA3D)
        {
            objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
            if (MyCamOper.Target.CompareTag(TagManager.WORLDPLANE))
                distance = cameraDistance * 0.3f * objectSize;
            else
            {
                //float cameraView = Mathf.Tan( Mathf.Deg2Rad * MyCam.fieldOfView); // Visible height 1 meter in front
                //Debug.Log(cameraView);
                //distance = cameraDistance * objectSize / cameraView; // Combined wanted distance from the object
                //distance -= 0.5f * objectSize;

                distance = cameraDistance + objectSize;
            }

            Vector3 max = targetObject.Mytr.position - distance * MyCam.transform.forward;
            max.y += bounds.center.y;
            MyCam.transform.position = max;

            MyCamOper.SetCameraPosition(max, MyCam.orthographicSize);
        }
        else if (MyMode == CameraMode.CAMERA2D)
        {
            objectSize = Mathf.Max(objectSizes.x, objectSizes.z);
            if (MyCamOper.Target.CompareTag(TagManager.WORLDPLANE))
                distance = (cameraDistance * objectSize * 0.15f);
            else
                distance = (cameraDistance * objectSize * 0.2f);
            MyCam.orthographicSize = distance;
            Vector3 pos = MyCam.transform.position;
            pos.x = targetObject.Mytr.position.x;
            pos.z = targetObject.Mytr.position.z;
            MyCam.transform.position = pos;

            MyCamOper.SetCameraPosition(pos, distance);
        }
    }

    Bounds GetBounds(Collider[] colls)
    {
        Bounds bounds = new Bounds();

        if (colls != null)
        {
            for (int i = 0; i < colls.Length; i++)
            {
                bounds.Encapsulate(colls[i].bounds.size);
            }
        }
        else
            Debug.LogError("Collider is null");
        //DrawBounds(bounds, 3.0f);
        return bounds;
    }

    void DrawBounds(Bounds b, float delay = 3)
    {
        // bottom
        var p1 = new Vector3(b.min.x, b.min.y, b.min.z);
        var p2 = new Vector3(b.max.x, b.min.y, b.min.z);
        var p3 = new Vector3(b.max.x, b.min.y, b.max.z);
        var p4 = new Vector3(b.min.x, b.min.y, b.max.z);

        Debug.DrawLine(p1, p2, UnityEngine.Color.blue, delay);
        Debug.DrawLine(p2, p3, UnityEngine.Color.red, delay);
        Debug.DrawLine(p3, p4, UnityEngine.Color.yellow, delay);
        Debug.DrawLine(p4, p1, UnityEngine.Color.magenta, delay);

        // top
        var p5 = new Vector3(b.min.x, b.max.y, b.min.z);
        var p6 = new Vector3(b.max.x, b.max.y, b.min.z);
        var p7 = new Vector3(b.max.x, b.max.y, b.max.z);
        var p8 = new Vector3(b.min.x, b.max.y, b.max.z);

        Debug.DrawLine(p5, p6, UnityEngine.Color.blue, delay);
        Debug.DrawLine(p6, p7, UnityEngine.Color.red, delay);
        Debug.DrawLine(p7, p8, UnityEngine.Color.yellow, delay);
        Debug.DrawLine(p8, p5, UnityEngine.Color.magenta, delay);

        // sides
        Debug.DrawLine(p1, p5, UnityEngine.Color.white, delay);
        Debug.DrawLine(p2, p6, UnityEngine.Color.gray, delay);
        Debug.DrawLine(p3, p7, UnityEngine.Color.green, delay);
        Debug.DrawLine(p4, p8, UnityEngine.Color.cyan, delay);
    }

    private void ChangeCameraMode(CameraMode mode)
    {
        if (MyMode == mode)
        {
            CameraVisible(true);
            ControlScenes.Inst.CurrentCameraMode = MyMode;
            ControlScenes.Inst.MainCam = MyCam;
            ControlScenes.Inst.ChangeCamera.Invoke();
        }
        else
        {
            CameraVisible(false);
        }

        //Event Create 카메라로 전송 필요
        OnChangePosition();
    }

    /// <summary>
    /// 모드 변경시
    /// </summary>
    /// <param name="eCreate"></param>
    public override void Send(EventCreateObject eCreate)
    {
        MyClass.Use = MyCam.enabled;
        if (MyMode == CameraMode.CAMERA2D)
        {
            MyClass.ZoomFactor = MyCam.orthographicSize;
        }

        eCreate.ObjectType = (int)MpxNaviWorkObject.ObjectType.CAMERA;
        eCreate.PType = Protocol.ProtocolType.EventCreateObject;
        MyClass.FullScreen = false;

        MyClass.Position = CreateMPXObject.Vector3ToPoint3(Mytr.position);
        MyClass.Rotation = CreateMPXObject.Vector3ToPoint3(Mytr.eulerAngles);
        MyClass.Size = CreateMPXObject.Vector3ToPoint3(Mytr.localScale);
        eCreate.ObjInfo = MyClass;

        base.Send(eCreate);
    }

    private void OnChangePosition()
    {
        EventCreateObject eCreate = new EventCreateObject();
        eCreate.Command = EventCreateObject.COMMAND_EDIT;
        Send(eCreate);
    }

    void CameraVisible(bool show)
    {
        MyCam.enabled = show;
        MyCamOper.enabled = show;
    }

    /// <summary>
    /// 오브젝트 삭제 순서에 따라 null값이 뜨므로 변경해야함
    /// </summary>
    private void OnDestroy()
    {
        //ControlScenes.Inst.GuiCameraView.OnClick.RemoveListener(ChangeCameraMode);
        //MPXObjectManager.Inst.ChangeSelect.RemoveListener(ChangeSelect);
    }
}
