using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTimeButton : MonoBehaviour
{
    public void Clicked()
    {
        StartCoroutine(Connecting.Client.GetTime(WriteTime));
    }

    private void OnApplicationFocus(bool focus)
    {
        if(focus)
        {
            StartCoroutine(Connecting.Client.GetTime(WriteTime));
        }
    }

    public void WriteTime(string time) => Debug.Log(time);
}
