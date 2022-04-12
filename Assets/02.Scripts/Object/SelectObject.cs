using MPXObject.NaviWorkObject;
using MPXRemote.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectObject : MonoBehaviour
{
    public MPXUnityObject ThisObject;
    Outline line;
    MPXWorldPlane world;
    [SerializeField]
    Vector2 Edge1;
    [SerializeField]
    Vector2 Edge2;

    [SerializeField]
    bool useSelect = false;

    private void Awake()
    {
        MPXObjectManager.Inst.ChangeSelect.AddListener(OnChangeSelect);
        ControlScenes.Inst.ChangeCameraMode.AddListener(ChangeCameraMode);
        MPXObjectManager.Inst.SetWorld.AddListener(SetWorld);
        ControlScenes.Inst.ChangeMode.AddListener(ChangeMode);
        ChangeMode(ControlScenes.Inst.CurrentMode);
        SetWorld();
    }

    private void ChangeMode(ModeManagement mode)
    {
        if (mode == ModeManagement.NONE || mode == ModeManagement.SIMULATION)
        {
            useSelect = true;
        }
        else
        {
            useSelect = false;
        }
    }

    private void SetWorld()
    {
        world = MPXObjectManager.Inst.GetWorldPlane();
        if (world != null)
        {
            Edge1 = new Vector2(world.Mytr.localScale.x, world.Mytr.localScale.z);
            Edge2 = new Vector2(-world.Mytr.localScale.x, -world.Mytr.localScale.z);
            Edge1 *= 0.5f;
            Edge2 *= 0.5f;
        }
    }

    private void ChangeCameraMode(CameraMode mode)
    {
        if (line != null)
        {
            if (mode == CameraMode.CAMERA2D)
            {
                line.OutlineWidth = 0.2f;
            }
            else if (mode == CameraMode.CAMERA3D)
            {
                line.OutlineWidth = 2.0f;
            }
        }
    }

    private void OnChangeSelect(MPXUnityObject obj)
    {
        if (ThisObject != obj)
        {
            line.enabled = false;
            IsSelect = false;
            if (ThisObject.CompareTag(TagManager.RAIL))
            {
                MPXRail rail = (MPXRail)ThisObject;
                rail.UnSelected();
            }
        }
        else
        {
            if (!IsSelect)
            {
                Select();
            }
        }
    }

    private void Start()
    {
        if (line == null)
        {
            line = ThisObject.SelectEffect;
            line.OutlineColor = Color.yellow;
            line.OutlineMode = Outline.Mode.OutlineVisible;
            ChangeCameraMode(ControlScenes.Inst.CurrentCameraMode);
        }
        if (line != null)
        {
            line.enabled = false;
            ChangeCameraMode(ControlScenes.Inst.CurrentCameraMode);
        }
    }

    Vector3 offset;
    [SerializeField]
    Vector3 originScreenPoint;
    private void OnMouseDown()
    {
        if (!useSelect)
            return;
        Camera main = ControlScenes.Inst.MainCam;
        originScreenPoint = main.WorldToScreenPoint(ThisObject.Mytr.position);
        offset = ThisObject.Mytr.position - main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, originScreenPoint.z));
    }


    public bool IsSelect = false;
    private void OnMouseUp()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (!useSelect)
            return;
        Select();
        SenderManager.Inst.SendSelectObject(ThisObject);
    }

    void Select()
    {
        if (IsSelect)
        {
            //MPXObjectManager.Inst.SetDefaultSelectObject.Invoke();
            //isSelect = false;
            //line.enabled = false;
        }
        else
        {
            IsSelect = true;
            line.enabled = true;
            if (ThisObject.CompareTag(TagManager.RAIL))
            {
                MPXRail rail = (MPXRail)ThisObject;
                rail.OnSelected();
            }
        }
        MPXObjectManager.Inst.AddObjectToList(ThisObject);
    }

    private Vector3 screenPoint;
    private void OnMouseDrag()
    {
        if (!useSelect)
            return;
        if (IsSelect)
        {
            Camera main = ControlScenes.Inst.MainCam;
            screenPoint = main.WorldToScreenPoint(ThisObject.Mytr.position);
            Vector3 cursorScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 cursorPosition = main.ScreenToWorldPoint(cursorScreenPoint) + offset;
            cursorPosition.y = ThisObject.Mytr.position.y;
            if (cursorPosition.x > Edge1.x)
            {
                cursorPosition.x = Edge1.x;
            }
            else if (cursorPosition.x < Edge2.x)
            {
                cursorPosition.x = Edge2.x;
            }
            if (cursorPosition.z > Edge1.y)
            {
                cursorPosition.z = Edge1.y;
            }
            else if (cursorPosition.z < Edge2.y)
            {
                cursorPosition.z = Edge2.y;
            }
            ThisObject.Mytr.position = cursorPosition;
        }
    }
}
