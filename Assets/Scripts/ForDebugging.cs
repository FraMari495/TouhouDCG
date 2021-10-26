using Position;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string s_player = true + ":" + string.Join(",", Field.I(true).Cards);
            string s_rival = false + ":" + string.Join(",", Field.I(false).Cards);

            Debug.Log(s_player);
            Debug.Log(s_rival);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {

        }

        }

}
