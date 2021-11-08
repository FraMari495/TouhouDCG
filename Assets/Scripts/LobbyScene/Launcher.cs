using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private AudioClip gameStart;
    [SerializeField] private int[] tutorialCard_player;
    [SerializeField] private int[] tutorialCard_rival;

    #region Singleton
    private static Launcher instance;

    public static Launcher Instance
    {
        get
        {
            Launcher[] instances = null;
            if (instance == null)
            {
                instances = FindObjectsOfType<Launcher>();
                if (instances.Length == 0)
                {
                    Debug.LogError("Launcherのインスタンスが存在しません");
                    return null;
                }
                else if (instances.Length > 1)
                {
                    Debug.LogError("Launcherのインスタンスが複数存在します");
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

    private void OnDestroy()
    {
        instance = null;
    }
    #endregion




    public const byte MaxPlayerPerRoom = 2;

    #region Private Serializable Fields
    #endregion

    #region Public Fields
    [Tooltip("接続、ゲーム開始ボタン")]
    [SerializeField] private GameObject controlPanel;
    [SerializeField] private GameObject progressLabel;
    #endregion

    #region Private Field
    private string gameVersion = "0.1";
    private bool isConnecting = false;
    private Coroutine coroutine = null;
    #endregion


    #region MonoBehaviour Callbacks
    private void Awake()
    {

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        if (!SoundManager.I.isPlaying)
        {
            SoundManager.I.Play(clip);
        }
        //progressLabel.SetActive(false);
        //controlPanel.SetActive(true);
    }
    #endregion

    #region Public method
    /// <summary>
    /// 接続プロセスを開始する
    /// - 既に接続済み        ランダムな部屋に入る
    /// - まだ接続していない    このアプリをphoton cliud networkに接続　
    /// </summary>
    public void Connect()
    {
        PhotonNetwork.OfflineMode = false;

        loadingPanel.SetActive(true);
        Debug.Log("iou");
        //progressLabel.SetActive(true);
        //controlPanel.SetActive(false);
        if (PhotonNetwork.IsConnected)
        {
            //接続済みならランダムな部屋に入る　部屋が無かったら作る
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            //photon online serverに接続
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;

        }
    }

    public void Cancel()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        PhotonNetwork.Disconnect();
        loadingPanel.SetActive(false);

    }


    public void MakeDeckButtonClicked()
    {
        WBTransition.SceneManager.LoadScene("MakeDeck");
    }



    private void VsCPU(bool tutorial)
    {
        PhotonNetwork.OfflineMode = true;

        List<int> myDeck = new List<int>();

        foreach (var item in SaveSystem.Instance.UserData.Deck)
        {
            for (int i = 0; i < item.Number; i++)
            {
                myDeck.Add(item.CardId);
            }
        }


        List<int> rivalDeck = new List<int>();


        foreach (var item in SaveSystem.Instance.UserData.Deck)
        {
            for (int i = 0; i < item.Number; i++)
            {
                rivalDeck.Add(item.CardId);
            }
        }



        SoundManager.I.FadeOut();

        if (tutorial)
        {
            WBTransition.SceneManager.LoadScene("GameScene",
               new Dictionary<string, object>() { { "offline", true }, { "myDeck", tutorialCard_player }, { "rivalDeck", tutorialCard_rival }, { "tutorial", true } },
               new Dictionary<string, object>() { { "offline", true }, { "tutorial", true } }
               );
        }
        else
        {
            WBTransition.SceneManager.LoadScene("GameScene",
               new Dictionary<string, object>() { { "offline", true }, { "myDeck", myDeck.ToArray() }, { "rivalDeck", rivalDeck.ToArray() }, { "tutorial", false } },
               new Dictionary<string, object>() { { "offline", true }, { "tutorial", false } }
               );
        }
    }
    public void VsCpuButton()
    {
        SoundManager.I.PlaySE(gameStart);

        VsCPU(false);
    }

    public void TutorialButton()
    {
        SoundManager.I.PlaySE(gameStart);

        VsCPU(true);
    }
    #endregion


    #region MonoBehabiour Pun Callbacks
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");

        if (!isConnecting) return;
        Debug.Log("OnConnectedToMaster() was called");
        PhotonNetwork.JoinRandomRoom();
        isConnecting = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("OnDisconnected() was called with reason {0}", cause);
        //progressLabel.SetActive(true);
        //controlPanel.SetActive(false);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed() was called. No random room available so we create one.\nCalling: PhotonNetwork.CreateRoom");
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = MaxPlayerPerRoom });
    }
   
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() was called. Now this client is in a room.");
        Debug.Log("!!!!!!!!!!!!!!" + PhotonNetwork.CurrentRoom.PlayerCount + "!!!!!!!!!!!!!!!!!");
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("we load the room");
            coroutine = StartCoroutine(WaitForRival());
        }
        else if(PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            int[] deck = SaveSystem.Instance.UserData.Deck.ConvertAll(c => c.CardId).ToArray();

            GoGameScene(deck);
        }
    }



    private IEnumerator WaitForRival()
    {
        yield return new WaitWhile(() => PhotonNetwork.CurrentRoom.PlayerCount < 2);
        List<int> myDeck = new List<int>();

        foreach (var item in SaveSystem.Instance.UserData.Deck)
        {
            for (int i = 0; i < item.Number; i++)
            {
                myDeck.Add(item.CardId);
            }
        }

        GoGameScene(myDeck.ToArray());
    }

    private void GoGameScene(int[] deck)
    {
        SoundManager.I.FadeOut();
        PhotonNetwork.IsMessageQueueRunning = false;
        WBTransition.SceneManager.LoadScene("GameScene",
            new Dictionary<string, object>() { {"offline",false }, { "myDeck", deck }, { "tutorial", false } },
            new Dictionary<string, object>() { { "offline", false } ,{ "tutorial",false } }
            );
    }


    #endregion
}
