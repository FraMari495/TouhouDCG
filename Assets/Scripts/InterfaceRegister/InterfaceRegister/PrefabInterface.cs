using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PrefabInterface<T>:ISerializationCallbackReceiver where T:class
{

    [SerializeField] private ScriptableObject scriptableObject;
    [SerializeField] private string typeName;

    public static implicit operator T(PrefabInterface<T> scriptableObject) => scriptableObject;

    public T GetInterface()
    {
        if (scriptableObject is T)
        {
            return scriptableObject as T;
        }
        else
        {
            return null;
        }
    }

    public void OnBeforeSerialize()
    {
        Check();
    }

    public void OnAfterDeserialize()
    {
        Check();
    }

    private void Check()
    {
        Debug.Log("‚¿‚¥‚Á‚­");

        if (typeName != typeof(T).ToString())
        {
            scriptableObject = null;
            typeName = typeof(T).ToString();
        }
    }
}
