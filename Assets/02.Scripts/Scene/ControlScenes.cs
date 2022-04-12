using System;
using System.Collections;
using System.Collections.Generic;
using MPXRemote.Message;
using MPXObject.NaviWorkObject;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControlScenes : Singleton<ControlScenes>
{
    public GUICameraView GuiCameraView;
    public FadeInOut Fade;
    public Camera IntroCamera;
    /// <summary>
    /// 테스트용
    /// </summary>
    //public SendEventCreateObjectMyself sendEvent;

    public UnityEvent OpenedScene = new UnityEvent();
    public UnityEvent ClosedScene = new UnityEvent();
    public UnityEvent AllSceneObjectsLoded = new UnityEvent();
    public UnityEvent OnCompleteCreateCamera = new UnityEvent();
    /// <summary>
    /// Destroy All Mpx Object
    /// Use "new file" or "open file"
    /// </summary>
    public class OnChangeCameraMode : UnityEvent<CameraMode> { }
    public OnChangeCameraMode ChangeCameraMode = new OnChangeCameraMode();

    public UnityEvent ChangeCamera = new UnityEvent();
    public UnityEvent DefaultFullView = new UnityEvent();


    public class OnChangeMode : UnityEvent<ModeManagement> { }
    public OnChangeMode ChangeMode = new OnChangeMode();


    public bool IsCreate2D;
    public bool IsCreate3D;
    public CameraMode CurrentCameraMode;
    [SerializeField]
    public ModeManagement CurrentMode;
    public Camera MainCam;

    public bool IsNew = true;

    const string WORK_SCENE = "WorkView";
    const string START_SCENE = "Main";

    public override void Init()
    {
        base.Init();
        ReceiverManager.Inst.OnReceiveScene.AddListener(OnReceive);
        ReceiverManager.Inst.OnReceiveScreen.AddListener(OnReceive);
        OnCompleteCreateCamera.AddListener(CreateCamera);
        ReceiverManager.Inst.OnReceiveChangeMode.AddListener(OnReceive);
    }

    private void CreateCamera()
    {
        if (IsCreate2D && IsCreate3D)
        {
            if (CurrentCameraMode==CameraMode.CAMERA3D)
                GuiCameraView.OnClick3D();
            else if (CurrentCameraMode==CameraMode.CAMERA2D)
                GuiCameraView.OnClick2D();
        }
    }

    private void OnReceive(EventScreen eventInfo)
    {
        SenderManager.Inst.EndProcess(eventInfo.ID);
        StartCoroutine(SendScreenResponse(eventInfo));
    }

    private void OnReceive(EventChangeMode mode)
    {
        switch (mode.Command)
        {
            //case EventChangeMode.COMMAND_NONE:
            //    break;
            case EventChangeMode.COMMAND_WALL:
                CurrentMode = ModeManagement.WALL;
                break;
            case EventChangeMode.COMMAND_FLOOR:
                CurrentMode = ModeManagement.FLOOR;
                break;
            case EventChangeMode.COMMAND_RAIL:
                CurrentMode = ModeManagement.RAIL;
                break;
            case EventChangeMode.COMMAND_SIMULATION:
                CurrentMode = ModeManagement.SIMULATION;
                break;
            default:
                CurrentMode = ModeManagement.NONE;
                break;
        }
        ChangeMode.Invoke(CurrentMode);
    }

    /// <summary>
    /// 화면이 완전히 가려지고/보여지고 난 뒤 응답하기 위함.
    /// </summary>
    /// <param name="eventInfo"></param>
    /// <returns></returns>
    IEnumerator SendScreenResponse(EventScreen eventInfo)
    {
        //if (Fade.IsRunFadeIn)
        //{
        //    Fade.StopCoroutine(Fade.Fadein);
        //    Fade.IsRunFadeIn = false;
        //    Fade.SetCoroutine();
        //}
        if (Fade.IsRunFadeOut)
        {
            Fade.StopCoroutine(Fade.Fadeout);
            Fade.IsRunFadeOut = false;
        }
        Fade.SetCoroutine();

        if (eventInfo.Command == EventScreen.COMMAND_SHOW)
            Fade.FadeIn();
        else if (eventInfo.Command == EventScreen.COMMAND_HIDE)
        {
            AllSceneObjectsLoded.Invoke();
            yield return Fade.StartCoroutine(Fade.Fadeout);
        }
    }

    private void OnReceive(EventScene eventInfo)
    {
        if (eventInfo.Command == EventScene.COMMAND_NEW)
        {
            IsNew = true;
            StartCoroutine(AddWork(eventInfo));
        }
        else if (eventInfo.Command == EventScene.COMMAND_OPEN)
        {
            IsNew = false;//새로 생성한 오브젝트 들이 모두 만들어 졌다면 isnew를 다시 true로 _추후수정20201219
            StartCoroutine(AddWork(eventInfo));
        }
        else if (eventInfo.Command == EventScene.COMMAND_CLOSE)
        {
            StartCoroutine(RemoveWork(eventInfo));
        }
    }

    IEnumerator AddWork(EventScene eventInfo)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(WORK_SCENE, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(WORK_SCENE));
        //오브젝트 생성(불러올)
        OpenedScene.Invoke();
        CurrentMode = ModeManagement.NONE;
        ChangeMode.Invoke(CurrentMode);
        SenderManager.Inst.EndProcess(eventInfo.ID);
        //yield return Fade.StartCoroutine(Fade.FadeOut());
    }

    void UIVisible(bool show)
    {
        GuiCameraView.gameObject.SetActive(show);
    }

    IEnumerator RemoveWork(EventScene eventInfo)
    {
        IsCreate2D = false;
        IsCreate3D = false;

        ReceiverManager.Inst.Clear();

        if (SenderManager.Inst.Count() > 0)
            SenderManager.Inst.RemoveExceptLastValue();
        if (SceneManager.GetActiveScene().name == WORK_SCENE)
        {
            //yield return Fade.StartCoroutine(Fade.FadeIn());
            //UIVisible(false);
            MPXObjectManager.Inst.Clear();
            IntroCamera.enabled = true;
            AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(WORK_SCENE);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(START_SCENE));
            ClosedScene.Invoke();
        }
        //SenderManager.Inst.ProcessStart();
        //ReceiverManager.Inst.ProcessStart();
        SenderManager.Inst.EndProcess(eventInfo.ID);
    }

    public IEnumerator AddWorkView()
    {
        if (SceneManager.GetActiveScene().name == WORK_SCENE)
        {
            yield return StartCoroutine(RemoveWorkView());
        }
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(WORK_SCENE, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(WORK_SCENE));
        //오브젝트 생성(불러올)

        //eventScreen이 hide 이면
        IntroCamera.enabled = false;
        yield return null;
        GuiCameraView.gameObject.SetActive(true);
        yield return Fade.StartCoroutine(Fade.Fadeout);
        //Fade.FadeOut();
    }

    IEnumerator RemoveWorkView()
    {
        //yield return Fade.StartCoroutine(Fade.Fadein);
        Fade.FadeIn();
        GuiCameraView.gameObject.SetActive(false);
        MPXObjectManager.Inst.Clear();
        IntroCamera.enabled = true;
        AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(WORK_SCENE);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(START_SCENE));
    }

    /// <summary>
    /// 가릴 이미지 구분
    /// </summary>
    //public void LoadingViewControl(EventScreen eventInfo)
    //{
    //        ControlImage(eventInfo.Command);
    //}

    /// <summary>
    /// 0:show 1:hide
    /// </summary>
    /// <param name="command"></param>
    public void ControlImage(int command)
    {
        if (command == 0)
        {
            IntroCamera.enabled = false;
            Fade.StartCoroutine(Fade.Fadeout);
            //Fade.FadeOut();
        }
        else if (command == 1)
        {
            IntroCamera.enabled = true;
            //Fade.StartCoroutine(Fade.Fadein);
            Fade.FadeIn();
        }
    }

    private void OnDestroy()
    {
        ReceiverManager.Inst.OnReceiveScene.RemoveListener(OnReceive);
        ReceiverManager.Inst.OnReceiveScreen.RemoveListener(OnReceive);
        OnCompleteCreateCamera.RemoveListener(CreateCamera);
    }
}