/**
 *  the author: D2og
 *  date: 2019-03-06
 *  what it does: lens control (mimic the Unity editor)
 *  how to use it: just put the script on the camera
 *  operation method:   1. Right click and press + mouse to move so that the lens to rotate
 *                      2. Press the mouse wheel + mouse to move so that the lens to translation
 *                      3. Right mouse button + keyboard w s a d (+leftShift) so that the lens to move
 *                      4. the mouse wheel rolling so that the lens forward and backward
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CameraOperate : MonoBehaviour
{
    [Tooltip("Mouse wheel rolling control lens please enter, the speed of the back")]
    [Range(0.5f, 2f)] public float scrollSpeed = 1f;
    [Tooltip("Right mouse button control lens X axis rotation speed")]
    [Range(0.0f, 2f)] public float rotateXSpeed = 1f;
    [Tooltip("Right mouse button control lens Y axis rotation speed")]
    [Range(0.0f, 2f)] public float rotateYSpeed = 1f;
    [Tooltip("Mouse wheel press, lens translation speed")]
    [Range(0.5f, 2f)] public float moveSpeed = 1f;
    [Tooltip("The keyboard controls how fast the camera moves")]
    [Range(0.5f, 2f)] public float keyMoveSpeed = 1f;

    //Whether the lens control operation is performed
    public bool operate = true;

    public bool Orthographic = false;

    //Whether keyboard control lens operation is performed
    public bool isKeyOperate = true;

    //Whether currently in rotation
    private bool isRotate = false;

    //Is currently in panning
    private bool isMove = false;

    //Camera transform component cache
    private Transform m_transform;

    //The initial position of the camera at the beginning of the operation
    private Vector3 traStart;

    //The initial position of the mouse as the camera begins to operate
    private Vector3 mouseStart;

    //Is the camera facing down
    private bool isDown = false;

    public Camera MyCam;
    public MPXUnityObject Target;

    [SerializeField]
    float DelayTime = 0.1f;
    [SerializeField]
    float time;


    public UnityEvent OnChangePosition = new UnityEvent();

    RaycastHit hit;
    Vector3 mos;
    Vector3 hitPos;

    /// <summary>
    /// world와 ignorelayer 무시
    /// </summary>
    private LayerMask IgnoreLayer = ~((1 << 2) | (1 << 9));

    public MPXWorldPlane worldObj;

    // Start is called before the first frame update
    void Start()
    {
        m_transform = transform;
        Orthographic = MyCam.orthographic;
        //ChangePosition = m_transform.position;
        //ChangeAngle = MyCam.orthographicSize;
    }

    public void SetCameraPosition(Vector3 point,float angle)
    {
        ChangePosition = point;
        ChangeAngle = angle;
    }

    Vector3 OriginPosition;
    Vector3 ChangePosition;
    float ChangeAngle;
    // Update is called once per frame
    void Update()
    {
        if (operate)
        {
            OriginPosition = MyCam.transform.position;
            time += Time.deltaTime;
            //When in the rotation state, and the right mouse button is released, then exit the rotation state
            if (isRotate && Input.GetMouseButtonUp(1))
            {
                //회전 변경
                OnChangePosition.Invoke();
                isRotate = false;
            }
            //When it is in the translation state, and the mouse wheel is released, it will exit the translation state
            if (isMove && Input.GetMouseButtonUp(2))
            {
                isMove = false;
                OnChangePosition.Invoke();
            }

            //Whether it's in a rotational state
            if (!Orthographic && isRotate)
            {
                //Gets the offset of the mouse on the screen
                Vector3 offset = Input.mousePosition - mouseStart;
                ///타겟기준 회전
                //if (Target != null)
                //{
                //    m_transform.RotateAround(Target.Mytr.position,
                //                        Vector3.up,
                //                        Input.GetAxis("Mouse X") * 10.0f);
                //    m_transform.RotateAround(Target.Mytr.position,
                //                                    Vector3.right,
                //                                    -Input.GetAxis("Mouse Y") * 10.0f);
                //}
                // whether the lens is facing down
                if (isDown)
                {
                    // the final rotation Angle = initial Angle + offset, 0.3f coefficient makes the rotation speed normal when rotateYSpeed, rotateXSpeed is 1
                    m_transform.rotation = Quaternion.Euler(traStart + new Vector3(offset.y * 0.3f * rotateYSpeed, -offset.x * 0.3f * rotateXSpeed, 0));
                }
                else
                {
                    // final rotation Angle = initial Angle + offset
                    m_transform.rotation = Quaternion.Euler(traStart + new Vector3(-offset.y * 0.3f * rotateYSpeed, offset.x * 0.3f * rotateXSpeed, 0));
                }

                // simulate the unity editor operation: right click, the keyboard can control the lens movement
                //if (isKeyOperate)
                //{
                //    float speed = keyMoveSpeed;
                //    // press LeftShift to make speed *2
                //    //按下LeftShift使得速度*2
                //    if (Input.GetKey(KeyCode.LeftShift))
                //    {
                //        speed = 2f * speed;
                //    }
                //    // press W on the keyboard to move the camera forward
                //    if (Input.GetKey(KeyCode.W))
                //    {
                //        m_transform.position += m_transform.forward * Time.deltaTime * 10f * speed;
                //    }
                //    // press the S key on the keyboard to back up the camera
                //    if (Input.GetKey(KeyCode.S))
                //    {
                //        m_transform.position -= m_transform.forward * Time.deltaTime * 10f * speed;
                //    }
                //    // press A on the keyboard and the camera will turn left
                //    if (Input.GetKey(KeyCode.A))
                //    {
                //        m_transform.position -= m_transform.right * Time.deltaTime * 10f * speed;
                //    }
                //    // press D on the keyboard to turn the camera to the right
                //    if (Input.GetKey(KeyCode.D))
                //    {
                //        m_transform.position += m_transform.right * Time.deltaTime * 10f * speed;
                //    }
                //    if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
                //    {
                //        OnChangePosition.Invoke();
                //        //ScalingWorldObject();
                //    }
                //}
            }
            // press the right mouse button to enter the rotation state
            else if (Input.GetMouseButtonDown(1))
            {
                // enter the rotation state
                isRotate = true;
                // record the initial position of the mouse in order to calculate the offset
                mouseStart = Input.mousePosition;
                // record the initial mouse Angle
                traStart = m_transform.rotation.eulerAngles;
                // to determine whether the lens is facing down (the Y-axis is <0 according to the position of the object facing up),-0.0001f is a special case when x rotates 90
                isDown = m_transform.up.y < -0.0001f ? true : false;
            }

            // whether it is in the translation state
            if (isMove)
            {
                // mouse offset on the screen
                Vector3 offset = Input.mousePosition - mouseStart;
                // final position = initial position + offset
                m_transform.position = traStart + m_transform.up * -offset.y * 0.1f * moveSpeed + m_transform.right * -offset.x * 0.1f * moveSpeed;
            }
            // click the mouse wheel to enter translation mode
            else if (Input.GetMouseButtonDown(2))
            {
                // translation begins
                isMove = true;
                // record the initial position of the mouse
                mouseStart = Input.mousePosition;
                // record the initial position of the camera
                traStart = m_transform.position;
            }

            // how much did the roller roll
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            // scroll to scroll or not
            if (scroll != 0)
            {
                if (Orthographic)
                {
                    MyCam.orthographicSize -= scroll * 1000f * Time.deltaTime * scrollSpeed;
                }
                else
                {
                    m_transform.position += m_transform.forward * scroll * 1000f * Time.deltaTime * scrollSpeed;
                }
                // position = current position + scroll amount
            }
            if (!CheckDistance())
            {
                if (Orthographic)
                {
                    MyCam.orthographicSize = angle;
                }
                else
                {
                    m_transform.position = OriginPosition;
                }
            }
            else
            {
                if (time >= DelayTime)
                {
                    if (Orthographic)
                    {
                        if (MyCam.orthographicSize != ChangeAngle)
                        {
                            OnChangePosition.Invoke();
                        }
                        ChangeAngle = MyCam.orthographicSize;
                    }
                    else
                    {
                        if (m_transform.position != ChangePosition)
                        {
                            OnChangePosition.Invoke();
                        }
                        ChangePosition = m_transform.position;
                    }
                    time = 0;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    mos = Input.mousePosition;
                    mos.z = MyCam.farClipPlane;

                    Ray ray = MyCam.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit, mos.z, IgnoreLayer))
                    {
                        hitPos = hit.point;
                        Debug.DrawRay(ray.origin, ray.direction * 500f, Color.blue, 5.0f);
                        //todo
                    }
                    else
                    {
                        MPXObjectManager.Inst.SetDefaultSelectObject.Invoke();
                    }
                }
            }
        }
    }

    Bounds bounds = new Bounds();
    public void GetWorldBound()
    {
        if (worldObj.MyCol != null)
        {
            for (int i = 0; i < worldObj.MyCol.Length; i++)
            {
                bounds.Encapsulate(worldObj.MyCol[i].bounds);
            }
        }
    }

    [SerializeField]
    float angle;
    [SerializeField]
    float objectSize;
    [SerializeField]
    float viewSize = 100.0f;//todo_20210119
    void CameraPositionLimit()
    {
        GetWorldBound();
        Vector3 objectSizes = bounds.max - bounds.min;
        objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
        objectSize = objectSize * 0.05f;
        if (Orthographic)
        {
            angle = Mathf.Clamp(MyCam.orthographicSize, objectSize, objectSize * viewSize);
        }
        else
        {
            angle = Mathf.Clamp(MyCam.transform.position.y, objectSize, objectSize * viewSize);
        }
    }

    bool CheckDistance()
    {
        CameraPositionLimit();
        float distance;
        if (Orthographic)
        {
            distance = MyCam.orthographicSize - worldObj.Mytr.position.y;
        }
        else
        {
            distance = MyCam.transform.position.y - worldObj.Mytr.position.y;
        }
        if (distance > objectSize && distance < objectSize * viewSize)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}


