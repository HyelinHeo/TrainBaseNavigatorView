using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MPXRemote;
using MPXRemote.Message;
using System;
using UnityEngine.Events;

public class ReceiverManager : Singleton<ReceiverManager>
{
    /// <summary>
    /// stack incomming data by communication
    /// </summary>
    Queue<Protocol> ReceiveQueue = new Queue<Protocol>();
    Queue<Protocol> ObjectQueue = new Queue<Protocol>();
    Queue<Protocol> responseQueue = new Queue<Protocol>();

    public class OnReceiveEventScene : UnityEvent<EventScene> { }
    public OnReceiveEventScene OnReceiveScene = new OnReceiveEventScene();
    public class OnReceiveEventScreen : UnityEvent<EventScreen> { }
    public OnReceiveEventScreen OnReceiveScreen = new OnReceiveEventScreen();

    public class OnReceiveEventCreateObject : UnityEvent<EventCreateObject> { }
    public OnReceiveEventCreateObject OnReceiveCreateObj = new OnReceiveEventCreateObject();

    public class OnReceiveEventProcess : UnityEvent<EventProcess> { }
    public OnReceiveEventProcess OnReceiveProcess = new OnReceiveEventProcess();

    public class OnReceiveEventChangeMode : UnityEvent<EventChangeMode> { }
    public OnReceiveEventChangeMode OnReceiveChangeMode = new OnReceiveEventChangeMode();

    Receiver receiver = new Receiver();


    public void StartReceive()
    {
        receiver.Init("main1");
        ProcessStart();
        receiver.OnReceive += ReceiverMain_OnReceive;
    }

    private void Update()
    {
        Protocol receive = Dequeue(ReceiveQueue);
        if (receive != null)
        {
            Message.Inst.AddMessage("receive : " + receive.ToString());
            CheckProtocolType(receive);
        }

        ///씬이 생성되었을 때만 오브젝트 생성하도록
        if (isOpen)
        {
            Protocol receiveObj = Dequeue(ObjectQueue);
            if (receiveObj != null)
                CreateEventObject(receiveObj);
        }

        Protocol response = Dequeue(responseQueue);
        if (response != null)
        {
            if (response.PType==Protocol.ProtocolType.EventCreateObject)
            {
                CreateEventObject(response);
            }
            Message.Inst.AddMessage("receive : " + response.ToString());
        }
    }

    public override void Init()
    {
        base.Init();
        ControlScenes.Inst.OpenedScene.AddListener(SceneOpend);
        ControlScenes.Inst.ClosedScene.AddListener(SceneClosed);
        isOpen = false;
    }

    bool isOpen;
    private void SceneClosed()
    {
        isOpen = false;
    }

    private void SceneOpend()
    {
        isOpen = true;
    }

    private void ReceiverMain_OnReceive(Protocol p)
    {
        if (p.Request == Protocol.TYPE_REQUEST)
        {
            Debug.Log(p);
            SenderManager.Inst.AddSendProtocol(p);
            Enqueue(ReceiveQueue, p);
        }
        else
        {
            Enqueue(responseQueue, p);
        }
    }

    /// <summary>
    /// protocoltype 별로 이벤트 발생_ 수정필요_20201209
    /// </summary>
    /// <param name="type"></param>
    public void CheckProtocolType(Protocol p)
    {
        if (p.Request == Protocol.TYPE_RESPONSE)
        {
            return;
        }
        switch (p.PType)
        {
            case Protocol.ProtocolType.None:
                break;
            case Protocol.ProtocolType.EventScene:
                EventScene eScene = new EventScene();
                eScene = (EventScene)p;
                OnReceiveScene.Invoke(eScene);
                break;
            case Protocol.ProtocolType.EventScreen:
                EventScreen eScreen = new EventScreen();
                eScreen = (EventScreen)p;
                OnReceiveScreen.Invoke(eScreen);
                break;
            case Protocol.ProtocolType.EventSelect://todo
                EventSelect eSelect = new EventSelect();
                eSelect = (EventSelect)p;
                if (eSelect.Command==EventSelect.COMMAND_CANCEL)
                {
                    MPXObjectManager.Inst.ReceiveUnSelect();
                }
                else
                {
                    MPXUnityObject mpxObj = MPXObjectManager.Inst.FindMPXObject(eSelect.ObjInfo.ID);
                    if (mpxObj.SelectEffect != null)
                    {
                        MPXObjectManager.Inst.ChangeSelect.Invoke(mpxObj);
                    }
                    else
                    {
                        MPXObjectManager.Inst.ReceiveUnSelect();
                    }
                }
                SenderManager.Inst.EndProcess(p.ID);
                break;
            case Protocol.ProtocolType.EventCreateObject:
                if (isOpen)
                    CreateEventObject(p);
                else
                    Enqueue(ObjectQueue, p);
                break;
            case Protocol.ProtocolType.EventSimulationSetting://todo
                SenderManager.Inst.EndProcess(p.ID);
                break;
            case Protocol.ProtocolType.EventChangeMode:
                EventChangeMode eMode = new EventChangeMode();
                eMode = (EventChangeMode)p;
                OnReceiveChangeMode.Invoke(eMode);
                SenderManager.Inst.EndProcess(p.ID);
                break;
            case Protocol.ProtocolType.EventInput:
                SenderManager.Inst.EndProcess(p.ID);
                break;

            case Protocol.ProtocolType.EventProcess:
                EventProcess ePro = new EventProcess();
                ePro = (EventProcess)p;
                OnReceiveProcess.Invoke(ePro);
                break;
            case Protocol.ProtocolType.EventDebug:
                EventDebug eDebug = new EventDebug();
                eDebug = (EventDebug)p;
                Message.Inst.ShowDebug(eDebug);
                break;
            default:
                SenderManager.Inst.EndProcess(p.ID);
                break;
        }
    }

    void CreateEventObject(Protocol p)
    {
        EventCreateObject eCreate = new EventCreateObject();
        eCreate = (EventCreateObject)p;
        OnReceiveCreateObj.Invoke(eCreate);
    }

    /// <summary>
    /// bring the protocol at queue
    /// </summary>
    /// <returns></returns>
    public Protocol Dequeue(Queue<Protocol> receiveP)
    {
        if (receiveP != null && receiveP.Count > 0)
        {
            return receiveP.Dequeue();
        }
        return null;
    }

    /// <summary>
    /// add protocol at queue
    /// </summary>
    /// <param name="p"></param>
    public void Enqueue(Queue<Protocol> receiveP, Protocol p)
    {
        if (receiveP == null)
        {
            receiveP = new Queue<Protocol>();
        }
        receiveP.Enqueue(p);
    }

    void Clear(Queue<Protocol> queue)
    {
        if (queue != null)
        {
            queue.Clear();
            queue = null;
        }
    }

    private void OnDestroy()
    {
        OnDestroyMain();
    }

    void OnDestroyMain()
    {
        if (receiver != null)
        {
            receiver.OnReceive -= ReceiverMain_OnReceive;
            receiver.Dispose();
        }
        Clear();
    }

    public void Clear()
    {
        Clear(ReceiveQueue);
        Clear(ObjectQueue);
        Clear(responseQueue);
    }

    public void ProcessStart()
    {
        receiver.Start();
    }
}
