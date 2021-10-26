using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviour�ȃN���X���V���O���g���ɂ���
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
                    Debug.LogError($"{typeof(T).Name}�̃C���X�^���X�����݂��܂���");
                    return null;
                }
                else if (instances.Length > 1)
                {
                    Debug.LogError($"{typeof(T).Name}�̃C���X�^���X���������݂��܂�");
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
