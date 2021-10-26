using Photon.Pun;
using Position;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WBTransition;
using WBTutorial;

public class SceneInitializer :MonoBehaviourPun, ISceneInitializer
{
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
        Debug.Log("�V�[���`�F���W����");
        PhotonNetwork.IsMessageQueueRunning = true;
        Debug.Log("�V�[���`�F���W����2");

        Time.timeScale = 1;
        //�J�ڑO�V�[������󂯎�������
        bool offline = (bool)args["offline"];//�I�t���C�����ۂ�
        int[] myDeck = (int[])args["myDeck"];//�����̃f�b�L
        int[] rivalDeck = offline ? (int[])args["rivalDeck"] : null;//�ΐ푊��̃f�b�L(offline�̂Ƃ��̂�)
        IsTutorial = (bool)args["tutorial"];
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
        Debug.Log("AfterOpenMask_initial");
        bool tutorial = (bool)args["tutorial"];
        if (!(bool)args["offline"])
        {

            //�I�����C�����[�h�̏ꍇ
            Coroutine coroutine = this.StartCoroutine(Prepare(tutorial));

        }
        else
        {
            StartCoroutine(Deck.I(true).DecidingInitialHand(tutorial));
            StartCoroutine(Deck.I(false).DecidingInitialHand(tutorial));
            if (tutorial)
            {
                Tutorial = GameObject.Instantiate(Resources.Load<GameObject>("TutorialObject"), GameObject.Find("Canvas").transform, false).GetComponent<TutorialManager>();
          
                Tutorial.StartTutorial(); 
            }
        }
        Debug.Log("AfterOpenMask_end");

    }

    /// <summary>
    /// �I�����C�����[�h�̏ꍇ�ɍs���鏀��
    /// </summary>
    /// <returns></returns>
    private IEnumerator Prepare(bool tutorial)
    {
        //�ڑ�����܂őҋ@
        yield return StartCoroutine(ConnectionManager.Instance.WaitForConnection());

        if (tutorial)
        {
            //�ΐ푊���3���h���[
            for (int i = 0; i < 3; i++) Deck.I(true).Draw();
            Tutorial.StartTutorial();
        }
        else
        {
            //������D������
            yield return Deck.I(true).StartMulligan();
        }

        //PlayableId(�J�[�h�̃C���X�^���X���Ƃɗ^������Id)�̃��X�g
        List<int> playableIds = new List<int>();

        //���ׂĂ�PlayableId�����X�g�ɒǉ�����
        Hand.I(true).Cards.ForEach(card => playableIds.Add((int)card.PlayableId));
        Deck.I(true).Cards.ForEach(card => playableIds.Add((int)card.PlayableId));

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
}
