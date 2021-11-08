using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ITest
{

}

public class InterfaceText : MonoBehaviour
{
    [SerializeField] private PrefabInterface<ITest> prefabs;
}
