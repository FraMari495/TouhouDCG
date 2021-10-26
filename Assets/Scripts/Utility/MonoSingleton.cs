using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviourなクラスをシングルトンにする
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class MonoSingleton<T> : MonoBehaviour where T:MonoBehaviour
{
    #region Singleton
    private static T instance;

    public static T I
    {
        get
        {
            T[] instances = null;
            if (instance == null)
            {
                instances = FindObjectsOfType<T>();
                if (instances.Length == 0)
                {
                    Debug.LogError($"{typeof(T).Name}のインスタンスが存在しません");
                    return null;
                }
                else if (instances.Length > 1)
                {
                    Debug.LogError($"{typeof(T).Name}のインスタンスが複数存在します");
                    return null;
                }
                else
                {
                    instance = instances[0];
                }
            }
            return instance;
        }
    }
    #endregion

    private void OnDestroy()
    {
        instance = null;
    }
}
