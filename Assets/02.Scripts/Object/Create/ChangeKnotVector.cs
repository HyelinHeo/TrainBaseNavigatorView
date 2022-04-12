using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeKnotVector : MonoBehaviour
{
    public MegaShape shape;
    [SerializeField]
    MegaKnot knot;
    public int MyIdx;
    public bool IsInvec;

    public Transform MyPoint;
    public Transform OtherPoint;
    public MPXRail ThisObject;

    public LineRenderer ThisLineRen;

    [SerializeField]
    Vector3 knotP;
    Camera main;

    private void Start()
    {
        knot = shape.splines[0].knots[MyIdx];
    }

    private void OnEnable()
    {
        EventInputSystem.Inst.ClickDownUI.AddListener(OnClickUI);
        EventInputSystem.Inst.OnClickUpUI.AddListener(OnClickUpUI);
    }

    private void OnClickUpUI()
    {
        isClicked = false;
    }

    private void OnDisable()
    {
        EventInputSystem.Inst.ClickDownUI.RemoveListener(OnClickUI);
        EventInputSystem.Inst.OnClickUpUI.RemoveListener(OnClickUpUI);
    }

    private void OnClickUI(Transform tr,Vector3 hitPos)
    {
        if (tr == MyPoint)
        {
            offset = MyPoint.position - hitPos;
            knotP = knot.p;
            StartCoroutine(SetKnotVector());
        }
    }

    [SerializeField]
    Vector3 offset;
    //[SerializeField]
    //Vector3 originScreenPoint;
    //private void OnMouseDown()
    //{
    //    Debug.Log("onMouseDown");
    //    //Camera main = ControlScenes.Inst.MainCam;
    //    main = Camera.main;
    //    originScreenPoint = main.WorldToScreenPoint(MyPoint.position);
    //    offset = MyPoint.position - main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, originScreenPoint.z));
    //    knotP = knot.p;
    //}

    Vector3 screenPoint;
    [SerializeField]
    Vector3 distance;
    private bool isClicked;

    /*private void OnMouseDrag()*/
    private void SetKnot()
    {
        //Camera main = ControlScenes.Inst.MainCam;
        main = Camera.main;
        screenPoint = main.WorldToScreenPoint(MyPoint.position);
        Vector3 cursorScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 cursorPosition = main.ScreenToWorldPoint(cursorScreenPoint) + offset;
        cursorPosition.y = MyPoint.position.y;
        MyPoint.position = cursorPosition;
        ThisLineRen.SetPosition(IsInvec ? 0 : 1, MyPoint.position);
        if (IsInvec)
        {
            knot.invec = MyPoint.localPosition;
        }
        else
        {
            knot.outvec = MyPoint.localPosition;
        }
        distance = knotP - MyPoint.localPosition/* + offset*/;
        distance.y = MyPoint.position.y;
        OtherPoint.localPosition = knotP + distance;
        ThisLineRen.SetPosition(IsInvec ? 1 : 0, OtherPoint.position);
        if (IsInvec)
        {
            knot.outvec = OtherPoint.localPosition;
        }
        else
        {
            knot.invec = OtherPoint.localPosition;
        }
        shape.ReCalcLength();
        shape.ResetMesh();
    }


    IEnumerator SetKnotVector()
    {
        isClicked = true;
        while (isClicked)
        {
            SetKnot();
            yield return null;
        }
        ThisObject.Modify();
    }
}
