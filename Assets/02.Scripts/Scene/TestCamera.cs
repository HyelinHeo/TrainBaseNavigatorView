using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCamera : MonoBehaviour
{
        /// <summary>
        /// 카메라 전체 보기
        /// targetCollider=worldPlane
        /// </summary>
    public Collider targetCollider;//선택한 오브젝트의 콜라이더(선택한 오브젝트가 없다면 월드 플레인이 중심)
    public Camera My3DCamera;
    public Camera My2DCamera;
    //public float elevation;
    public float cameraDistance = 2.0f;
    [Range(0.0f, 1.0f)]
    public float ZoomFactor;
    public void Change3DCameraPosition()
    {
        Vector3 objectSizes = targetCollider.bounds.max - targetCollider.bounds.min;
        float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
        float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * My3DCamera.fieldOfView); // Visible height 1 meter in front
        float distance = cameraDistance * objectSize / cameraView; // Combined wanted distance from the object
        //distance = distance - objectSize;
        distance -= 0.5f * objectSize; // Estimated offset from the center to the outside of the object
        Vector3 max = targetCollider.bounds.center - distance * My3DCamera.transform.forward;
        Debug.Log(GetRatePerVector(targetCollider.transform.position, max, My3DCamera.transform.position));
        My3DCamera.transform.position = Vector3.Lerp(targetCollider.transform.position, max, ZoomFactor);
        Debug.Log(GetRatePerVector(targetCollider.transform.position, max, My3DCamera.transform.position));
        //MyCamera.transform.rotation = Quaternion.Euler(new Vector3(elevation, 0, 0));
    }

    public void Change2DCameraPosition()
    {
        Vector3 objectSizes = targetCollider.bounds.max - targetCollider.bounds.min;
        float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
        float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * My2DCamera.fieldOfView); // Visible height 1 meter in front
        Debug.Log(cameraView);
        float distance = cameraDistance * objectSize / cameraView; // Combined wanted distance from the object
        distance = distance - objectSize;
        //distance += 0.5f * objectSize; // Estimated offset from the center to the outside of the object
        Debug.Log(GetRatePer(targetCollider.transform.position.y, distance, My2DCamera.orthographicSize));
        My2DCamera.orthographicSize = Mathf.Lerp(targetCollider.transform.position.y, distance, ZoomFactor);
        Debug.Log(GetRatePer(targetCollider.transform.position.y, distance, My2DCamera.orthographicSize));
    }

    /// <summary>
    /// 두 벡터 사이에 위치한 벡터의 퍼센트 찾기
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="target"></param>
    /// <returns></returns>
   float GetRatePerVector(Vector3 min,Vector3 max, Vector3 target) {
        float origin = Vector3.Distance(max, min);
        float tg = Vector3.Distance(target, min);
        float per = tg / origin;
   return per;
    }

    /// <summary>
    /// 두 점 사이에 위치한 점의 퍼센트 찾기
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    float GetRatePer(float min, float max, float target)
    {
        float origin = max - min;
        float tg = target - min;
        float per = tg / origin;
        return per;
    }
}
