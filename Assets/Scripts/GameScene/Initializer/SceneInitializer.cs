using Photon.Pun;
using Position;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WBTransition;
using WBTutorial;

internal class SceneInitializer :MonoBehaviourPun, ISceneInitializer
{
    [SerializeField] private ChoicingPanel choicingPanel;

    public TutorialManager Tutorial { get; private set; }
    public bool IsTutorial { get; private set; }


    private Coroutine coroutine;

    /// <summary>
    /// �}�X�N���J���n�߂�O�ɌĂ΂�郁�\�b�h
    /// </summary>
    /// <param name="args">�I�t���C�����[�h���ۂ��A�f�b�L�̏��</param>
    /// <returns></returns>
    public IEnumerator BeforeOpenMask(Dictionary<string, object> args)
    {
        PhotonNetwork.IsMessageQueueRunning = true;

        Time.timeScale = 1;
        //�J�ڑO�V�[������󂯎�������
        bool offline = (bool)args["offline"];//�I�t���C�����ۂ�
        int[] myDeck = (int[])args["myDeck"];//�����̃f�b�L
        int[] rivalDeck = offline ? (int[])args["rivalDeck"] : null;//�ΐ푊��̃f�b�L(offline�̂Ƃ��̂�)
        IsTutorial = (bool)args["tutorial"];
        ReactivePropertyList.I.Tutorial = IsTutorial;
        if (!offline)
        {
            ReactivePropertyList.I.FirstAttack = PhotonNetwork.IsMasterClient;
        }
        else
        {
            ReactivePropertyList.I.FirstAttack = true;
        }

        Debug.Log("�I�t���C�����O");

        PhotonNetwork.OfflineMode = offline;
        Debug.Log("�I�t���C������");

        if (!IsTutorial)
        {
            myDeck = myDeck.Shuffle().ToArray();
            if(offline) rivalDeck = rivalDeck.Shuffle().ToArray();
        }



        //�f�b�L�̐���
        Position.Deck.I(true).MakeDeck(myDeck);
        if(offline)Position.Deck.I(false).MakeDeck(rivalDeck);
        Debug.Log("MakeDeck����");

        yield return null;
    }



    /// <summary>
    /// �}�X�N���J����������ɌĂ΂�郁�\�b�h
    /// </summary>
    /// <param name="args"></param>
    public void AfterOpenMask(Dictionary<string, object> args)
    {
        bool tutorial = (bool)args["tutorial"];
        if (!(bool)args["offline"])
        {

            //�I�����C�����[�h�̏ꍇ
            Coroutine coroutine = this.StartCoroutine(Prepare());

        }
        else
        {
            StartCoroutine(DecidingInitialHand(tutorial,true));
            StartCoroutine(DecidingInitialHand(tutorial,false));
            if (tutorial)
            {
                Tutorial = GameObject.Instantiate(Resources.Load<GameObject>("TutorialObject"), GameObject.Find("Canvas").transform, false).GetComponent<TutorialManager>();
          
                Tutorial.StartTutorial();

                RivalInputManager.I.OfflineRival = Tutorial;
            }
            else
            {
                RivalInputManager.I.OfflineRival = new AI();
            }

            //ReactivePropertyList.I.GameStart(true, tutorial);

        }

    }

    /// <summary>
    /// �I�����C�����[�h�̏ꍇ�ɍs���鏀��
    /// </summary>
    /// <returns></returns>
    private IEnumerator Prepare()
    {
        //�ڑ�����܂őҋ@
        yield return StartCoroutine(ConnectionManager.Instance.WaitForConnection());
        //������D������
        yield return StartMulligan(true);

        //PlayableId(�J�[�h�̃C���X�^���X���Ƃɗ^������Id)�̃��X�g
        List<int> playableIds = new List<int>();

        //���ׂĂ�PlayableId�����X�g�ɒǉ�����
        Hand.I(true).Cards.ForEach(card => playableIds.Add((int)card.PlayableCardId));
        Deck.I(true).Cards.ForEach(card => playableIds.Add((int)card.PlayableCardId));

        //�ڑ�����܂őҋ@
        yield return StartCoroutine(ConnectionManager.Instance.WaitForConnection());


            //�f�b�L����������(�}���K���őI������3�����f�b�L�̈�ԏ�ɔz�u�����)
        yield return ConnectionManager.Instance.ExchangeDeck(Deck.I(true).InitialDeck, playableIds.ToArray());
        

        //�ΐ푊���3���h���[
        for (int i = 0; i < 3; i++) Deck.I(false).Draw();

        //�Q�[���X�^�[�g(����ARoomMaster����U)
        if (PhotonNetwork.IsMasterClient) ConnectionManager.Instance.GameStartFirstAttack();
    }


    /// <summary>
    /// �I�����C�����[�h�̏ꍇ�̃f�b�L�̃V���b�t��
    /// </summary>
    /// <param name="deck"></param>
    private void DeckShuffleForOnline(int[] deck )
    {
        //�I�t���C�����[�h������
        ForDebugging.I.Offline = false;

        //RPC()��Timescale = 0�ł͓����Ȃ�����
        Time.timeScale = 1;
        PhotonNetwork.IsMessageQueueRunning = true;

        //�����̃f�b�L���V���b�t��
        deck = deck.OrderBy(i => Guid.NewGuid()).ToArray();

        //�f�b�L�̐���
        Position.Deck.I(true).MakeDeck(deck);
    }


    private void OnDestroy()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }




    /// <summary>
    /// ������D�̌�����͂��߂郁�\�b�h
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartMulligan(bool isPlayer)
    {
        //�I����
        List<IPlayable> options = new List<IPlayable>();

        //�f�b�L�̏ォ��6����I�����Ƃ���
        for (int i = 0; i < 6; i++) options.Add(Deck.I(isPlayer).Cards[i]);

        //�I��������3���I������̂�҂�
        yield return Mulligan(options);
    }


    /// <summary>
    /// �f�b�L���\����������
    /// 6���I��ŕ\�����A3����I��
    /// </summary>
    /// <param name="myTurn"></param>
    /// <param name="selectedCards"></param>
    /// <returns></returns>
    private IEnumerator Mulligan(List<IPlayable> options)
    {
        //�}���K���̊J�n
        //�I�����ɁADecidedFiestHand()���\�b�h�����s�����
        choicingPanel.gameObject.SetActive(true);
        yield return choicingPanel.StartSelecting(options, 3, Deck.I(true).DecidedFiestHand);
    }


    /// <summary>
    /// �I�t���C���̏ꍇ�ɑ΂���
    /// ������D�̌���
    /// </summary>
    /// <returns></returns>
    private IEnumerator DecidingInitialHand(bool tutorial,bool isPlayer)
    {
        //�V�[���J�ڌア���Ȃ菉����D�I�����J�n����̂������
        yield return new WaitForSeconds(1);

        if (isPlayer)
        {
            //�v���C���[�͏�����D��3���I������
            if (!tutorial) yield return StartMulligan(isPlayer);
            else
            {
                // ChoicingPanel.I.gameObject.SetActive(false);
            }
            ReactivePropertyList.I.GameStart(true,false);
        }
        else
        {
            //���C�o���͎R�D�̏ォ��3����������D�Ƃ���
            //��X���C�o�����}���K�����s���悤�ɉ���
            if (tutorial) Deck.I(isPlayer).Draw();
            else for (int i = 0; i < 3; i++) Deck.I(isPlayer).Draw();
        }

    }


}
