using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOutline : MonoBehaviour
{
    public Outline outline;
    public MPXUnityObject ThisObj;

    private void Start()
    {
        outline.enabled = false;
    }

    private void OnMouseDown()
    {
        if (!Input.GetKeyDown(KeyCode.LeftControl))
        {
            MPXObjectManager.Inst.RemoveAllList();
            //모든 오브젝트 unselect이벤트.invoke();
        }
        MPXObjectManager.Inst.AddObjectToList(ThisObj);
        outline.enabled = true;
    }
}
