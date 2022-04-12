using System;
using System.Collections;
using System.Collections.Generic;
using MPXRemote.Message;
using UnityEngine;
using UnityEngine.Events;
using MPXObject;
using MPXCommon;

public class FileManager : Singleton<FileManager>
{
    const int NEW_FILE = 0;
    const int OPEN_FILE = 1;
    const int SAVE_FILE = 2;
    const string WORLD_ID = "1";
    const string WORLD_NAME = "WorldPlane";

    bool isDone = false;

    /// <summary>
    /// 진행율 (0~1)
    /// </summary>
    [Range(0.0f, 1.0f)]
    float ProgressValue;

    FadeInOut Fade;

    //void Start()
    //{
    //    Fade = SceneSwitchManager.Inst.Fade;
    //}

    //private void OnReceive(EventFile eFile)
    //{
    //    if (eFile.Request != SenderManager.REQUEST)
    //        return;
    //    if (eFile.Command==NEW_FILE)
    //    {
    //        StartCoroutine(NewFileMPX(eFile));
    //    }
    //    else if (eFile.Command == OPEN_FILE)
    //    {
    //        StartCoroutine(OpenFileMPX(eFile));
    //    }
    //    else if (eFile.Command == SAVE_FILE)
    //    {
    //        StartCoroutine(SaveFileMPX(eFile));
    //    }

    //}

    ///// <summary>
    ///// Create new file
    ///// 새 파일 생성(코루틴)_수정중
    ///// </summary>
    ///// <param name="eFile"></param>
    ///// <returns></returns>
    //public IEnumerator NewFileMPX(EventFile eFile)
    //{
    //     //MPXObjectManager.Inst.DestroyAllMPX.Invoke();

    //    //yield return StartCoroutine(MPXObjectManager.Inst.RemoveObjectAll());

    //    Vector3 size = new Vector3(eFile.Size.X, 1.0f, eFile.Size.Y);

    //    //월드 오브젝트 생성
    //    EventCreateObject cObj = new EventCreateObject();
    //    cObj.ObjectType = (int)MpxNaviWorkObject.ObjectType.WORLD_OBJECT;
    //    cObj.Request = SenderManager.REQUEST;
    //    ObjectInfo mpxObj = new ObjectInfo();
    //    mpxObj.ID = WORLD_ID;
    //    mpxObj.Name = WORLD_NAME;
    //    mpxObj.Position = CreateMPXObject.Vector3ToPoint3(Vector3.zero);
    //    mpxObj.Rotation = CreateMPXObject.Vector3ToPoint3(Vector3.zero);
    //    mpxObj.Size = CreateMPXObject.Vector3ToPoint3(size);
    //    cObj.ObjInfo = mpxObj;
    //    MPXObjectManager.Inst.CreateObject.Invoke(cObj);

    //    //카메라 생성

    //    //Directional light 생성

    //    yield return null;
    //    eFile.Request = SenderManager.RESPONSE;
    //    eFile.Progress = 1.0f;
    //    SenderManager.Inst.Send(eFile);
    //    Debug.Log(eFile.ToString());
    //    Fade.StartCoroutine(Fade.FadeOut());
    //    isDone = false;
    //}

    ///// <summary>
    ///// open file which exist fliepath
    ///// 경로에 있는 파일 열기(코루틴)
    ///// </summary>
    ///// <param name="eFile"></param>
    ///// <returns></returns>
    //private string OpenFileMPX(EventFile eFile)
    //{
    //    throw new NotImplementedException();
    //}

    ///// <summary>
    ///// Save file at filepath
    ///// 경로에 파일 저장(코루틴)
    ///// </summary>
    ///// <param name="eFile"></param>
    ///// <returns></returns>
    //private string SaveFileMPX(EventFile eFile)
    //{
    //    throw new NotImplementedException();
    //}


}
