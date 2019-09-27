using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FloatVariable : ScriptableObject, ISerializationCallbackReceiver
{
    public float initialValue;

    [NonSerialized] public float runtimeValue;

    public void OnAfterDeserialize()
    {
        runtimeValue = initialValue;
    }

    public void OnBeforeSerialize() { }
}
