using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMPXList<T>
{
    int Count { get; }

    void Add(T item);
    void Clear();
    bool Remove(T item);
}
