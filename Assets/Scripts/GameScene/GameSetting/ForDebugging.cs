
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForDebugging : MonoSingleton<ForDebugging>
{
    [SerializeField] private bool debuggingMode;
    public bool Offline { get; set; } = true;
    public bool DebugMode { get => debuggingMode; set => debuggingMode = value; }

    private void Start()
    {
        if(Offline)
        {
            //Photon.Pun.PhotonNetwork.OfflineMode = true;
        }
        if (debuggingMode)
        {
            //this.GetComponent<GUIConsole>().Show();
        }
    }



}
