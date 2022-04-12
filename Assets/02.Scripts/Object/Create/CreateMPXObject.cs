using System;
using System.Collections;
using System.Collections.Generic;
using MPXRemote.Message;
using UnityEngine;
using MPXObject;
using MPXObject.NaviWorkObject;
using MPXObject.NaviObject;
using UnityEngine.Events;

public enum ModeManagement
{
    NONE = -1,
    WALL = 0,
    FLOOR = 1,
    RAIL = 2,
    SIMULATION = 3
}

public class OnClickWorld : UnityEvent<Vector3> { }

public class CreateMPXObject : Singleton<CreateMPXObject>
{
    MUOLoader muoLoader;
    public OnClickWorld OnClickWorld = new OnClickWorld();
    //public Transform ParentWorld;
    //public Transform ParentFoundation;
    //public Transform ParentSimulation;
    //public MPXCameras ParentCameras;

    public override void Init()
    {
        base.Init();
        //ParentWorld = CreateChild(WORLD_PLANE);
        //ParentFoundation = CreateChild(FOUNDATION);
        //ParentSimulation = CreateChild(OBJECT);
        //ParentCameras = CreateChild(CAMERAS).gameObject.AddComponent<MPXCameras>;

        MUOLoader.Inst.Init();

        ReceiverManager.Inst.OnReceiveCreateObj.AddListener(OnReceive);

        muoLoader = MUOLoader.Inst;
        muoLoader.CompleteLoad.AddListener(CompleteLoad);
    }

    private void CompleteLoad(MPXSimulationImport obj, EventCreateObject eCreate)
    {
        if (obj == null)
            SenderManager.Inst.EndErrorProcess(eCreate.ID);
        else
        {
            obj.Init();
            obj.Draw(eCreate);
            MPXObjectManager.Inst.AddObjectToDic(obj.ID, obj);
        }
    }

    Transform CreateChild(string objName)
    {
        Transform tr = new GameObject(objName).transform;
        tr.SetParent(transform);

        return tr;
    }

    private void CreateObj(EventCreateObject obj)
    {
        MpxNaviWorkObject.ObjectType objType = (MpxNaviWorkObject.ObjectType)obj.ObjectType;
        if (objType != MpxNaviWorkObject.ObjectType.FOUNDATION_OBJECT
            && objType != MpxNaviWorkObject.ObjectType.SIMULATION_OBJECT)
        {
            MPXUnityObject newObj = InstantiateObject(objType.ToString());

            Draw(newObj, obj);
        }
        else if (objType == MpxNaviWorkObject.ObjectType.SIMULATION_OBJECT)//코드 정리 필요
        {
            MpxSimulationObject sObj = (MpxSimulationObject)obj.ObjInfo;

            if (sObj.MyObject.MyType == MpxNaviObject.ObjectType.NORMAL)
            {
                MpxNaviObjectNormal nObj = (MpxNaviObjectNormal)sObj.MyObject;
                MPXUnityObject newObj = InstantiateObject(nObj.PrimitiType.ToString());

                Draw(newObj, obj);
            }
            else if (sObj.MyObject.MyType == MpxNaviObject.ObjectType.IMPORT)
            {
                MpxNaviObjectImport iObj = (MpxNaviObjectImport)sObj.MyObject;
                string path = iObj.FilePath;

                if (iObj.Type == MpxNaviObjectImport.FileType.MUO)
                {
                    muoLoader.LoadFile(path, obj);
                }
                else
                {
                    //자체 포맷 외의 확장자
                    CompleteLoad(null, obj);
                }
            }
        }
        else if (objType == MpxNaviWorkObject.ObjectType.FOUNDATION_OBJECT)
        {
            MpxFoundationObject fObj = (MpxFoundationObject)obj.ObjInfo;
            MPXUnityObject newObj;
            if (fObj.Type==FoundationType.Wall)
            {
                 newObj = InstantiateObject("MPXWall");
                Draw(newObj, obj);
            }
            else if (fObj.Type==FoundationType.Rail)
            {
                newObj = InstantiateObject("MPXRail");
                Draw(newObj, obj);
            }
            else if (fObj.Type == FoundationType.Plane)
            {
                newObj = InstantiateObject("MPXFloor");
                Draw(newObj, obj);
            }
            else
            {
                SenderManager.Inst.EndErrorProcess(obj.ID);
            }
        }
    }

    void Draw(MPXUnityObject obj, EventCreateObject eCreate)
    {
        obj.Init(ControlScenes.Inst.IsNew);
        obj.Draw(eCreate);
        obj.EndProcess(eCreate);
        MPXObjectManager.Inst.AddObjectToDic(obj.ID, obj);
    }

    MPXUnityObject InstantiateObject(string path)
    {
        GameObject go = Instantiate(Resources.Load("Prefabs/" + path)) as GameObject;
        return go.GetComponent<MPXUnityObject>();
    }

    public void OnReceive(EventCreateObject eCreate)
    {
        if (eCreate.Command == EventCreateObject.COMMAND_CREATE)
        {
            MPXUnityObject mpxObj = MPXObjectManager.Inst.FindMPXObject(eCreate.ObjInfo.ID);
            
            if (mpxObj == null)
            {
                CreateObj(eCreate);
            }
            else
            {
                if (eCreate.Request==Protocol.TYPE_RESPONSE)
                {
                    mpxObj.Modify(eCreate);
                }
                else
                {
                    SenderManager.Inst.EndErrorProcess(eCreate.ID);
                    Debug.LogErrorFormat("[{0}], Object already exist : Object already exist Dictionary define 'CurrentObjects' Check MPXObjectManager.cs", eCreate.ObjInfo.Name);
                }
            }
        }
        else
        {
            MPXUnityObject mpxObj = MPXObjectManager.Inst.FindMPXObject(eCreate.ObjInfo.ID);
            if (mpxObj != null)
            {
                if (eCreate.Command == EventCreateObject.COMMAND_EDIT)
                {
                    mpxObj.Modify(eCreate);
                    mpxObj.EndProcess(eCreate);
                }
                else if (eCreate.Command == EventCreateObject.COMMAND_DELETE)
                    mpxObj.Delete(eCreate);
            }
            else
            {
                SenderManager.Inst.EndErrorProcess(eCreate.ID);
                Debug.LogErrorFormat("[{0}], Don't Modify this Object : Object does not exist Dictionary define 'CurrentObjects' Check MPXObjectManager.cs", eCreate.ObjInfo.Name);
            }
        }
    }

    /// <summary>
    /// Point3 Axis Transform Vector3
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public static Vector3 Point3ToVector3(Point3 point)
    {
        return new Vector3(point.X, point.Y, point.Z);
    }

    /// <summary>
    /// Vector3 Axis Transform Point3
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Point3 Vector3ToPoint3(Vector3 vector)
    {
        return new Point3(vector.x, vector.y, vector.z);
    }

    /// <summary>
    /// Point2 Axis Transform Vector2
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public static Vector2 Point2ToVector2(Point2 point)
    {
        return new Vector2(point.X, point.Y);
    }

    /// <summary>
    /// Vector2 Axis Transform Point2
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Point2 Vector2ToPoint2(Vector2 vector)
    {
        return new Point2(vector.x, vector.y);
    }

    private void OnDestroy()
    {
        ReceiverManager.Inst.OnReceiveCreateObj.RemoveListener(OnReceive);
    }
}
