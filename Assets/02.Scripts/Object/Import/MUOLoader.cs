using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using MPXObject;
using MPXFile;
using MPXRemote.Message;
using MpxUnityObject;

public class MUOLoader : MeshLoader<MUOLoader>
{
    public override void Init()
    {
        base.Init();
    }

    public override void LoadFile(string filePath, EventCreateObject eCreate = null)
    {
        base.LoadFile(filePath, eCreate);

        MPXSimulationImport obj = Load<MPXSimulationImport>(filePath);
        if (obj != null)
        {
            obj.AddSelectEffect();
            MPXUnityObjectChild[] children = obj.GetComponentsInChildren<MPXUnityObjectChild>();
            if (children != null)
            {
                obj.Children = new List<MPXUnityObjectChild>(children);
                for (int i = 0; i < obj.Children.Count; i++)
                {
                    obj.Children[i].AddSelectObject(obj);
                }
            }
            CompleteLoad.Invoke(obj, eCreate);
        }
        else
        {
            CompleteLoad.Invoke(null, eCreate);
        }
    }



    public T Load<T>(string filePath) where T : MPXUnityObject
    {
        MpxUnityObjectFile file = MPXFileManager.Load<MpxUnityObjectFile>(filePath);
        if (file != null)
        {
            T newObj = MpxUnityObjectFile.ToGameObject<T>(file);

            return newObj;
        }
        return null;
    }


}
