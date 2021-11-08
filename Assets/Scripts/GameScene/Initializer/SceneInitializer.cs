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
    /// マスクが開き始める前に呼ばれるメソッド
    /// </summary>
    /// <param name="args">オフラインモードか否か、デッキの情報</param>
    /// <returns></returns>
    public IEnumerator BeforeOpenMask(Dictionary<string, object> args)
    {
        PhotonNetwork.IsMessageQueueRunning = true;

        Time.timeScale = 1;
        //遷移前シーンから受け取った情報
        bool offline = (bool)args["offline"];//オフラインか否か
        int[] myDeck = (int[])args["myDeck"];//自分のデッキ
        int[] rivalDeck = offline ? (int[])args["rivalDeck"] : null;//対戦相手のデッキ(offlineのときのみ)
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
        bool tutorial = (bool)args["tutorial"];
        if (!(bool)args["offline"])
        {

            //オンラインモードの場合
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
    /// オンラインモードの場合に行われる準備
    /// </summary>
    /// <returns></returns>
    private IEnumerator Prepare()
    {
        //接続するまで待機
        yield return StartCoroutine(ConnectionManager.Instance.WaitForConnection());
        //初期手札を決定
        yield return StartMulligan(true);

        //PlayableId(カードのインスタンスごとに与えられるId)のリスト
        List<int> playableIds = new List<int>();

        //すべてのPlayableIdをリストに追加する
        Hand.I(true).Cards.ForEach(card => playableIds.Add((int)card.PlayableCardId));
        Deck.I(true).Cards.ForEach(card => playableIds.Add((int)card.PlayableCardId));

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




    /// <summary>
    /// 初期手札の決定をはじめるメソッド
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartMulligan(bool isPlayer)
    {
        //選択肢
        List<IPlayable> options = new List<IPlayable>();

        //デッキの上から6枚を選択肢とする
        for (int i = 0; i < 6; i++) options.Add(Deck.I(isPlayer).Cards[i]);

        //選択肢から3枚選択するのを待つ
        yield return Mulligan(options);
    }


    /// <summary>
    /// デッキを構成したあと
    /// 6枚選んで表示し、3枚を選ぶ
    /// </summary>
    /// <param name="myTurn"></param>
    /// <param name="selectedCards"></param>
    /// <returns></returns>
    private IEnumerator Mulligan(List<IPlayable> options)
    {
        //マリガンの開始
        //終了時に、DecidedFiestHand()メソッドが実行される
        choicingPanel.gameObject.SetActive(true);
        yield return choicingPanel.StartSelecting(options, 3, Deck.I(true).DecidedFiestHand);
    }


    /// <summary>
    /// オフラインの場合に対する
    /// 初期手札の決定
    /// </summary>
    /// <returns></returns>
    private IEnumerator DecidingInitialHand(bool tutorial,bool isPlayer)
    {
        //シーン遷移後いきなり初期手札選択を開始するのを避ける
        yield return new WaitForSeconds(1);

        if (isPlayer)
        {
            //プレイヤーは初期手札を3枚選択する
            if (!tutorial) yield return StartMulligan(isPlayer);
            else
            {
                // ChoicingPanel.I.gameObject.SetActive(false);
            }
            ReactivePropertyList.I.GameStart(true,false);
        }
        else
        {
            //ライバルは山札の上から3枚を初期手札とする
            //後々ライバルもマリガンを行うように改良
            if (tutorial) Deck.I(isPlayer).Draw();
            else for (int i = 0; i < 3; i++) Deck.I(isPlayer).Draw();
        }

    }


}
