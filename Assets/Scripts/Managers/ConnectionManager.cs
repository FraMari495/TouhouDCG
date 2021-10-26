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
                    Debug.LogError("ConnectionManager�̃C���X�^���X�����݂��܂���");
                    return null;
                }
                else if (instances.Length > 1)
                {
                    Debug.LogError("ConnectionManager�̃C���X�^���X���������݂��܂�");
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
    /// ��U or ��U
    /// ����ARoom Master����U
    /// </summary>
    public bool IsFirstAttack => PhotonNetwork.IsMasterClient;

    /// <summary>
    /// �I�t���C�����[�h���ۂ�
    /// </summary>
    public bool OfflineMode => PhotonNetwork.OfflineMode;

    #region public methods
    public void CloseRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    /// �ڑ��m�F
    /// </summary>
    /// <param name="onConnected">�ڑ��������ɍs������</param>
    /// <returns></returns>
    public IEnumerator WaitForConnection(Action onConnected = null)
    {
        //�ڑ��m�F�O�ɁAConnected��false�ɂ���(�ڑ��ɐ���������true�ƂȂ�)
        Connected = false;

        //�m�F������܂� or 20�b�o�܂Őڑ��Ƀg���C����
        for (int i = 0; i < 20; i++)
        {
            //1�b���ƂɃ��b�Z�[�W�𑗐M
            photonView.RPC(nameof(Response), RpcTarget.Others);
            yield return new WaitForSecondsRealtime(1);

            //�ڑ��ɐ���������I��
            if (Connected)
            {
                if(onConnected!=null) onConnected();
                yield break;
            }
        }

        Debug.Log("�ڑ����ؒf����܂���");
        
    }

    /// <summary>
    /// �����f�b�L�� Deck.I�ɓo�^����
    /// </summary>
    /// <returns></returns>
    public IEnumerator ExchangeDeck(int[] myDeck,int[] playableIds)
    {
        myPlayableIds = playableIds;
        if (myPlayableIds == null)
        {
            Debug.LogError("playableIds��null�ł�");
            throw new NotImplementedException();
        }
        if (IsFirstAttack)
        {
            Debug.Log("����̉������m�F���܂�");
            yield return WaitForConnection();
            Debug.Log("�m�F����");

            Debug.Log("�}�X�^�[�ł��B�f�b�L��n���܂�");
            photonView.RPC(nameof(ReceivedDeck), RpcTarget.Others, new object[2] { myDeck, playableIds });

        }
        else
        {
            Debug.Log("�}�X�^�[�ł͂���܂���B�f�b�L���n�����̂�҂��܂�");
        }

        yield return new WaitWhile(() => RivalDeck == null || RivalPlayables == null);
        Debug.Log(string.Join(",", RivalPlayables));
        Debug.Log("�f�b�L���󂯎��܂���");

        Position.Deck.I(false).MakeConnectedRivalDeck(RivalDeck, RivalPlayables);

    }

    /// <summary>
    /// ��U���g�p
    /// �������U�ɂ��āA�Q�[���J�n
    /// </summary>
    public void GameStartFirstAttack()
    {
        TurnManager.I.GameStart(true);
       // IsFirstAttack = true;
        photonView.RPC(nameof(SetInitialTurn), RpcTarget.Others);
    }

    /// <summary>
    /// ���������R�}���h���V���A�������A�ΐ푊��ɑ��M����
    /// </summary>
    /// <param name="command"></param>
    public void SendCommand(Command.Command command)
    {
        //�R�}���h���V���A����
        string commandJson = JsonUtility.ToJson(command);

        //�R�}���h�ɂ���āA�p����RPC���\�b�h��ύX
        string methodName = command switch
        {
            Command_TurnEnd turnEnd => nameof(ReceiveTurnEndCommand),
            Command_Attack attack => nameof(ReceiveAttackCommand),
            Command_CardPlay attack => nameof(ReceivePlayCommand),
            Command_UseBombCard userBombCard=> nameof(BombCommand),
            _ => throw new NotImplementedException()
        };

        //�ʐM�J�n
        photonView.RPC(methodName, RpcTarget.Others,commandJson );
    }

    /// <summary>
    /// �ʐM�����TurnManagewr.Judge()���Ă�
    /// </summary>
    public void Judge()
    {
        photonView.RPC(nameof(ReceiveJudge), RpcTarget.Others);
    }

    #endregion





    /*   private methods   */

    #region WaitForConnection()�p��private���\�b�h
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

    #region ExchangeDeck()�p��private���\�b�h
    private int[] RivalDeck { get; set; } = null;
    private int[] RivalPlayables { get; set; } = null;
    private int[] myPlayableIds;

    /// <summary>
    /// �}�X�^�[����f�b�L���󂯎��
    /// </summary>
    /// <param name="deckData"></param>
    [PunRPC]private void ReceivedDeck(object[] deckData)
    {
        if((int[])deckData[0] == null)
        {
            Debug.LogError("�}�X�^�[����󂯎�����f�b�L��null�ł�");
        }

        if ((int[])deckData[1] == null)
        {
            Debug.LogError("�}�X�^�[����󂯎�����f�b�L��PlayableIds��null�ł�");
        }

        Debug.Log("�}�X�^�[����f�b�L���󂯎��܂����B");
        RivalDeck = (int[])deckData[0];
        RivalPlayables = (int[])deckData[1];
        Debug.Log("�}���K�����I�������ɁA�}�X�^�[�Ƀf�b�L��n���܂�");

        StartCoroutine(WaitForFinishMulligan());

    }

    /// <summary>
    /// �}���K���I����҂��A�}�X�^�[�Ƀf�b�L��n��
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForFinishMulligan()
    {
        //�}���K���I����҂�
        yield return new WaitWhile(()=> myPlayableIds==null);

        //�}�X�^�[�Ƀf�b�L��n��
        int[] myDeck = Deck.I(true).InitialDeck;
        photonView.RPC(nameof(ReceiveResponceDeck), RpcTarget.Others, new object[2] { myDeck, myPlayableIds });
    }

    /// <summary>
    /// �}�X�^�[���f�b�L���󂯎��
    /// </summary>
    /// <param name="deckData"></param>
    [PunRPC]private void ReceiveResponceDeck(object[] deckData)
    {
        Debug.Log("�}�X���[���A�f�b�L���󂯎��܂���");
        if ((int[])deckData[0] == null)
        {
            Debug.LogError("�󂯎�����f�b�L��null�ł�");
        }

        if ((int[])deckData[1] == null)
        {
            Debug.LogError("�󂯎�����f�b�L��PlayableIds��null�ł�");
        }
        RivalDeck = (int[])deckData[0];
        RivalPlayables = (int[])deckData[1];
    }
    #endregion

    #region �^�[���p�̃��\�b�h

    /// <summary>
    /// ��U�̒[�����Ă�(��U�̒[�������s����)
    /// �Q�[�����J�n����
    /// </summary>
    [PunRPC] private void SetInitialTurn()
    {
        //��U�Ƃ���
     //   IsFirstAttack = false;

        //�Q�[�����J�n����
        TurnManager.I.GameStart(false);
    }
    #endregion

    #region Command���MRPC
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

    #region Judge�̒ʒmRPC
    [PunRPC]
    private void ReceiveJudge()
    {
        TurnManager.I.StartJudge(true);
    }
    #endregion
}
