using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Position;
using DG.Tweening;
using UniRx;

public class TurnManager : MonoSingleton<TurnManager>
{
    [SerializeField] private AudioClip intro;
    [SerializeField] private AudioClip main;
    [SerializeField] private TurnEndButton buttonImage;
    [SerializeField] private SceneInitializer sceneInitializer;
    [SerializeField] private ChoicingPanel choicingPanel;    

    private bool FirstAttack => ConnectionManager.Instance.IsFirstAttack;
    private bool firstTurn = true;

    /// <summary>
    /// ターンチェンジを通知
    /// </summary>
    public Subject<bool> NewTurnNotif { get; } = new Subject<bool>();

    /// <summary>
    /// ジャッジ開始
    /// </summary>
    public Subject<Unit> Judge { get; } = new Subject<Unit>();


    /// <summary>
    /// プレイヤーのターンか否か
    /// </summary>
    private bool turn;

    /// <summary>
    /// ターン
    /// </summary>
    public bool Turn
    {
        get => turn;
        set
        {
            turn = value;

            //ターンチェンジのテキストアニメーション
            AnimationManager.I.AddSequence(()=>AnimationMaker.TurnEndAnimation(buttonImage,value), "ターンチェンジ");

            //先攻の1ターン目はドローできない
            bool canDraw = true;
            if (firstTurn)
            {
                firstTurn = false;
                canDraw = false;
            }

            //ターンチェンジを通知
            NewTurnNotif.OnNext(value);

            //オフラインモードで相手のターンの場合、AIを動かす
            if (ConnectionManager.Instance.OfflineMode && !value)
            {
                if(sceneInitializer.Tutorial==null) AI.I.StartAI(canDraw,sceneInitializer.IsTutorial);
                else sceneInitializer.Tutorial.RunRivalCommand();
            }

            //ユーザーの入力の直前
            StartJudge(false);
        }
    }

    /// <summary>
    /// ターンチェンジのテキストアニメーション & ターンチェンジ
    /// </summary>
    /// <param name="isPlayer"></param>
    private void TurnChangeAnimation(bool isPlayer)
    {
        AnimationManager.I.AddSequence(() => DOTween.Sequence().AppendCallback(() => Turn = isPlayer),"ターンチェンジ");
    }

    /// <summary>
    /// ターンチェンジ
    /// </summary>
    /// <returns></returns>
    public bool ChangeTurn()
    {
        //ターン終了のスキルを発動
        Field.I(Turn).OnTurnEnd();

        //ターンチェンジのアニメーション & ターンチェンジ
        TurnChangeAnimation(!Turn);

        return true;
    }

    /// <summary>
    /// ゲーム開始
    /// </summary>
    /// <param name="myTurn"></param>
    public void GameStart(bool myTurn)
    {
        //ターンチェンジのアニメーション & ターンチェンジ
        TurnChangeAnimation(myTurn);

        //BGMの再生
        SoundManager.I.Play(intro,main);
    }

    /// <summary>
    /// デッキを構成したあと
    /// 6枚選んで表示し、3枚を選ぶ
    /// </summary>
    /// <param name="myTurn"></param>
    /// <param name="selectedCards"></param>
    /// <returns></returns>
    public IEnumerator Mulligan(List<IPlayable> options)
    {
        //マリガンの開始
        //終了時に、DecidedFiestHand()メソッドが実行される
        choicingPanel.gameObject.SetActive(true);
        yield return choicingPanel.StartSelecting(options, 3, Deck.I(true).DecidedFiestHand);
    }

    /// <summary>
    /// !!!ユーザーの入力直前に必ず呼ぶ!!!
    /// 死亡判定、プレイ可能なカードの判定、アタッカーの選択肢等
    /// </summary>
    public void StartJudge(bool rpc)
    {
        if(!rpc)ConnectionManager.Instance.Judge();
        Judge.OnNext(Unit.Default);
    }

}
