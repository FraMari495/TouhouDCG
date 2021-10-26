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
    /// マスクが開き始める前に呼ばれるメソッド
    /// </summary>
    /// <param name="args">オフラインモードか否か、デッキの情報</param>
    /// <returns></returns>
    public IEnumerator BeforeOpenMask(Dictionary<string, object> args)
    {
        Debug.Log("シーンチェンジ直後");
        PhotonNetwork.IsMessageQueueRunning = true;
        Debug.Log("シーンチェンジ直後2");

        Time.timeScale = 1;
        //遷移前シーンから受け取った情報
        bool offline = (bool)args["offline"];//オフラインか否か
        int[] myDeck = (int[])args["myDeck"];//自分のデッキ
        int[] rivalDeck = offline ? (int[])args["rivalDeck"] : null;//対戦相手のデッキ(offlineのときのみ)
        IsTutorial = (bool)args["tutorial"];
        Debug.Log("オフライン直前");

        PhotonNetwork.OfflineMode = offline;
        Debug.Log("オフライン直後");

        if (!IsTutorial)
        {
            myDeck = myDeck.Shuffle().ToArray();
            if(offline) rivalDeck = rivalDeck.Shuffle().ToArray();
        }



        //デッキの生成
        Position.Deck.I(true).MakeDeck(myDeck);
        if(offline)Position.Deck.I(false).MakeDeck(rivalDeck);
        Debug.Log("MakeDeck直後");

        yield return null;
    }



    /// <summary>
    /// マスクが開ききった後に呼ばれるメソッド
    /// </summary>
    /// <param name="args"></param>
    public void AfterOpenMask(Dictionary<string, object> args)
    {
        Debug.Log("AfterOpenMask_initial");
        bool tutorial = (bool)args["tutorial"];
        if (!(bool)args["offline"])
        {

            //オンラインモードの場合
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
    /// オンラインモードの場合に行われる準備
    /// </summary>
    /// <returns></returns>
    private IEnumerator Prepare(bool tutorial)
    {
        //接続するまで待機
        yield return StartCoroutine(ConnectionManager.Instance.WaitForConnection());

        if (tutorial)
        {
            //対戦相手は3枚ドロー
            for (int i = 0; i < 3; i++) Deck.I(true).Draw();
            Tutorial.StartTutorial();
        }
        else
        {
            //初期手札を決定
            yield return Deck.I(true).StartMulligan();
        }

        //PlayableId(カードのインスタンスごとに与えられるId)のリスト
        List<int> playableIds = new List<int>();

        //すべてのPlayableIdをリストに追加する
        Hand.I(true).Cards.ForEach(card => playableIds.Add((int)card.PlayableId));
        Deck.I(true).Cards.ForEach(card => playableIds.Add((int)card.PlayableId));

        //接続するまで待機
        yield return StartCoroutine(ConnectionManager.Instance.WaitForConnection());


            //デッキを交換する(マリガンで選択した3枚がデッキの一番上に配置される)
        yield return ConnectionManager.Instance.ExchangeDeck(Deck.I(true).InitialDeck, playableIds.ToArray());
        

        //対戦相手は3枚ドロー
        for (int i = 0; i < 3; i++) Deck.I(false).Draw();

        //ゲームスタート(現状、RoomMasterが先攻)
        if (PhotonNetwork.IsMasterClient) ConnectionManager.Instance.GameStartFirstAttack();
    }


    /// <summary>
    /// オンラインモードの場合のデッキのシャッフル
    /// </summary>
    /// <param name="deck"></param>
    private void DeckShuffleForOnline(int[] deck )
    {
        //オフラインモードを解除
        ForDebugging.I.Offline = false;

        //RPC()はTimescale = 0では動かないため
        Time.timeScale = 1;
        PhotonNetwork.IsMessageQueueRunning = true;

        //自分のデッキをシャッフル
        deck = deck.OrderBy(i => Guid.NewGuid()).ToArray();

        //デッキの生成
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
