using Command;
using Photon.Pun;
using Position;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviourPun
{
    #region Singleton
    private static ConnectionManager instance;

    public static ConnectionManager Instance
    {
        get
        {
            ConnectionManager[] instances = null;
            if (instance == null)
            {
                instances = FindObjectsOfType<ConnectionManager>();
                if (instances.Length == 0)
                {
                    Debug.LogError("ConnectionManagerのインスタンスが存在しません");
                    return null;
                }
                else if (instances.Length > 1)
                {
                    Debug.LogError("ConnectionManagerのインスタンスが複数存在します");
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

    /// <summary>
    /// 先攻 or 後攻
    /// 現状、Room Masterが先攻
    /// </summary>
    public bool IsFirstAttack => PhotonNetwork.IsMasterClient;

    /// <summary>
    /// オフラインモードか否か
    /// </summary>
    public bool OfflineMode => PhotonNetwork.OfflineMode;

    #region public methods
    public void CloseRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    /// 接続確認
    /// </summary>
    /// <param name="onConnected">接続成功時に行う処理</param>
    /// <returns></returns>
    public IEnumerator WaitForConnection(Action onConnected = null)
    {
        //接続確認前に、Connectedをfalseにする(接続に成功したらtrueとなる)
        Connected = false;

        //確認が取れるまで or 20秒経つまで接続にトライする
        for (int i = 0; i < 20; i++)
        {
            //1秒ごとにメッセージを送信
            photonView.RPC(nameof(Response), RpcTarget.Others);
            yield return new WaitForSecondsRealtime(1);

            //接続に成功したら終了
            if (Connected)
            {
                if(onConnected!=null) onConnected();
                yield break;
            }
        }

        Debug.Log("接続が切断されました");
        
    }

    /// <summary>
    /// 初期デッキを Deck.Iに登録する
    /// </summary>
    /// <returns></returns>
    public IEnumerator ExchangeDeck(int[] myDeck,int[] playableIds)
    {
        myPlayableIds = playableIds;
        if (myPlayableIds == null)
        {
            Debug.LogError("playableIdsがnullです");
            throw new NotImplementedException();
        }
        if (IsFirstAttack)
        {
            Debug.Log("相手の応答を確認します");
            yield return WaitForConnection();
            Debug.Log("確認完了");

            Debug.Log("マスターです。デッキを渡します");
            photonView.RPC(nameof(ReceivedDeck), RpcTarget.Others, new object[2] { myDeck, playableIds });

        }
        else
        {
            Debug.Log("マスターではありません。デッキが渡されるのを待ちます");
        }

        yield return new WaitWhile(() => RivalDeck == null || RivalPlayables == null);
        Debug.Log(string.Join(",", RivalPlayables));
        Debug.Log("デッキを受け取りました");

        Position.Deck.I(false).MakeConnectedRivalDeck(RivalDeck, RivalPlayables);

    }

    /// <summary>
    /// 先攻が使用
    /// 相手を後攻にして、ゲーム開始
    /// </summary>
    public void GameStartFirstAttack()
    {
        TurnManager.I.GameStart(true);
       // IsFirstAttack = true;
        photonView.RPC(nameof(SetInitialTurn), RpcTarget.Others);
    }

    /// <summary>
    /// 生成したコマンドをシリアル化し、対戦相手に送信する
    /// </summary>
    /// <param name="command"></param>
    public void SendCommand(Command.Command command)
    {
        //コマンドをシリアル化
        string commandJson = JsonUtility.ToJson(command);

        //コマンドによって、用いるRPCメソッドを変更
        string methodName = command switch
        {
            Command_TurnEnd turnEnd => nameof(ReceiveTurnEndCommand),
            Command_Attack attack => nameof(ReceiveAttackCommand),
            Command_CardPlay attack => nameof(ReceivePlayCommand),
            Command_UseBombCard userBombCard=> nameof(BombCommand),
            _ => throw new NotImplementedException()
        };

        //通信開始
        photonView.RPC(methodName, RpcTarget.Others,commandJson );
    }

    /// <summary>
    /// 通信相手のTurnManagewr.Judge()を呼ぶ
    /// </summary>
    public void Judge()
    {
        photonView.RPC(nameof(ReceiveJudge), RpcTarget.Others);
    }

    #endregion





    /*   private methods   */

    #region WaitForConnection()用のprivateメソッド
    private bool Connected { get; set; } = false;
    [PunRPC]private void Response()
    {
        photonView.RPC(nameof(ReceiveResponce), RpcTarget.Others);
    }
    [PunRPC]private void ReceiveResponce()
    {
        Connected = true;
    }
    #endregion

    #region ExchangeDeck()用のprivateメソッド
    private int[] RivalDeck { get; set; } = null;
    private int[] RivalPlayables { get; set; } = null;
    private int[] myPlayableIds;

    /// <summary>
    /// マスターからデッキを受け取る
    /// </summary>
    /// <param name="deckData"></param>
    [PunRPC]private void ReceivedDeck(object[] deckData)
    {
        if((int[])deckData[0] == null)
        {
            Debug.LogError("マスターから受け取ったデッキがnullです");
        }

        if ((int[])deckData[1] == null)
        {
            Debug.LogError("マスターから受け取ったデッキのPlayableIdsがnullです");
        }

        Debug.Log("マスターからデッキを受け取りました。");
        RivalDeck = (int[])deckData[0];
        RivalPlayables = (int[])deckData[1];
        Debug.Log("マリガンが終わった後に、マスターにデッキを渡します");

        StartCoroutine(WaitForFinishMulligan());

    }

    /// <summary>
    /// マリガン終了を待ち、マスターにデッキを渡す
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForFinishMulligan()
    {
        //マリガン終了を待つ
        yield return new WaitWhile(()=> myPlayableIds==null);

        //マスターにデッキを渡す
        int[] myDeck = Deck.I(true).InitialDeck;
        photonView.RPC(nameof(ReceiveResponceDeck), RpcTarget.Others, new object[2] { myDeck, myPlayableIds });
    }

    /// <summary>
    /// マスターがデッキを受け取る
    /// </summary>
    /// <param name="deckData"></param>
    [PunRPC]private void ReceiveResponceDeck(object[] deckData)
    {
        Debug.Log("マスがーが、デッキを受け取りました");
        if ((int[])deckData[0] == null)
        {
            Debug.LogError("受け取ったデッキがnullです");
        }

        if ((int[])deckData[1] == null)
        {
            Debug.LogError("受け取ったデッキのPlayableIdsがnullです");
        }
        RivalDeck = (int[])deckData[0];
        RivalPlayables = (int[])deckData[1];
    }
    #endregion

    #region ターン用のメソッド

    /// <summary>
    /// 先攻の端末が呼ぶ(後攻の端末が実行する)
    /// ゲームを開始する
    /// </summary>
    [PunRPC] private void SetInitialTurn()
    {
        //後攻とする
     //   IsFirstAttack = false;

        //ゲームを開始する
        TurnManager.I.GameStart(false);
    }
    #endregion

    #region Command送信RPC
    [PunRPC]
    private void ReceiveTurnEndCommand(string commandJson)
    {
        var command = ScriptableObject.CreateInstance(typeof(Command.Command_TurnEnd)) as Command.Command_TurnEnd;
        JsonUtility.FromJsonOverwrite(commandJson, command);
        Command.CommandManager.I.RPCRun(command);
    }

    [PunRPC]
    private void ReceivePlayCommand(string commandJson)
    {
        var command = ScriptableObject.CreateInstance(typeof(Command.Command_CardPlay)) as Command.Command_CardPlay;
        JsonUtility.FromJsonOverwrite(commandJson, command);
        Command.CommandManager.I.RPCRun(command);
    }

    [PunRPC]
    private void ReceiveAttackCommand(string commandJson)
    {
        var command = ScriptableObject.CreateInstance(typeof(Command.Command_Attack)) as Command.Command_Attack;
        JsonUtility.FromJsonOverwrite(commandJson, command);
        Command.CommandManager.I.RPCRun(command);
    }

    [PunRPC]
    public void BombCommand(string commandJson)
    {
        var command = ScriptableObject.CreateInstance(typeof(Command.Command_UseBombCard)) as Command.Command_UseBombCard;
        JsonUtility.FromJsonOverwrite(commandJson, command);
        Command.CommandManager.I.RPCRun(command);
    }
    #endregion

    #region Judgeの通知RPC
    [PunRPC]
    private void ReceiveJudge()
    {
        TurnManager.I.StartJudge(true);
    }
    #endregion
}
