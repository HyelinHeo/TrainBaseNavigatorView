using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MPXRemote;
using MPXRemote.Message;
using System;
using UnityEngine.EventSystems;

public class EventInputSystem : Singleton<EventInputSystem>
{
    const int MOUSE = 0;
    const int KEYBOARD = 1;
    const int MOUSE_LEFT = 0;
    const int MOUSE_RIGHT = 1;
    const int MOUSE_WHEEL = 2;
    const int KEY_VALUE = 3;

    public class OnClickDownUI : UnityEvent<Transform,Vector3> { }
    public OnClickDownUI ClickDownUI = new OnClickDownUI();
    public UnityEvent OnClickUpUI = new UnityEvent();

    RaycastHit hit;
    RaycastHit[] hits;
    Vector3 pos;
    Vector3 mos;
    float distanceRay;
    public Vector3 hitPos;
    public Camera MainCam;

    private LayerMask worldLayer = (1 << 9);
    //private LayerMask ignoreLayer = ~((1 << 2) | (1 << 0));
    private LayerMask UILayer = (1 << 5);

    public override void Init()
    {
        base.Init();
        ControlScenes.Inst.ChangeCamera.AddListener(ChangeCamera);
    }

    private void ChangeCamera()
    {
        if (ControlScenes.Inst.MainCam != null)
        {
            MainCam = ControlScenes.Inst.MainCam;
        }
    }


    bool isUI = false;
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetMouseButton(0))
            {
                Send(MOUSE, MOUSE_LEFT);
            }
            else if (Input.GetMouseButton(1))
            {
                Send(MOUSE, MOUSE_RIGHT);
            }
            else if (Input.GetMouseButton(2))
            {
                Send(MOUSE, MOUSE_WHEEL);
            }
            else
            {
                //Send(KEYBOARD, Input.GetKey(KeyCode.)
                Send(KEYBOARD, KEY_VALUE);
                //Send(KEYBOARD, Input.inputString);
            }
        }

        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (MainCam != null)
        {
            mos = Input.mousePosition;
            distanceRay = MainCam.farClipPlane;

            Ray ray = MainCam.ScreenPointToRay(mos);
            //if (Physics.Raycast(ray, out hit, distanceRay, ignoreLayer))
            {
                hits = Physics.RaycastAll(ray.origin, ray.direction, distanceRay);
                if (Input.GetMouseButtonDown(0))
                {
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (hits[i].collider.CompareTag(TagManager.UI))
                        {
                            isUI = true;
                            ClickDownUI.Invoke(hits[i].transform, hits[i].point);
                        }
                    }
                }

                if (isUI)
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        isUI = false;
                        OnClickUpUI.Invoke();
                        return;
                    }
                    return;
                }
                
                if (Physics.Raycast(ray, out hit, distanceRay, worldLayer))
                {
                    hitPos = hit.point;
                    if (Input.GetMouseButtonDown(0))
                    {
                        Debug.DrawRay(ray.origin, ray.direction * 500f, Color.red, 5.0f);
                        CreateMPXObject.Inst.OnClickWorld.Invoke(hitPos);
                    }
                }
            }
        }
    }

    void Send(int command, int key)
    {
        EventInput input = new EventInput();
        input.PType = Protocol.ProtocolType.EventInput;
        input.Command = command;
        input.Key = key;
        //todo
        SenderManager.Inst.SendRequest(input);
    }
}
