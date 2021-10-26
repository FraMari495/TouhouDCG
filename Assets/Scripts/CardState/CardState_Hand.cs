using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Position;

/// <summary>
/// カードが手札に存在するときに有効になるステート
/// </summary>
public abstract class CardState_Hand : CardState
{
    protected bool Special { get; private set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="playable">関連付けたいIPlayable</param>
    /// <param name="costText">コストを表示するテキスト</param>
    public CardState_Hand(IPlayable playable,bool special) : base(playable)
    {
        Special = special;
        CanvasGroup = Trn.GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// 手札にある時、自分のカードのみ表示
    /// </summary>
    public override bool Showing => IsPlayer;

    public override PosEnum Pos => PosEnum.Hand;

    /// <summary>
    /// ドラッグ中は、blocksRaycastsをfalseとする
    /// </summary>
    private CanvasGroup CanvasGroup { get; }

    public override bool Enter()
    {

        if (!Special) AnimationManager.I.AddSequence<AnimationManager.ToHand>(() => AnimationMaker.DrawAnimation(Playable), "ドロー");
        else AnimationManager.I.AddSequence<AnimationManager.ToHand>(() => AnimationMaker.SpecialSummonAnimation_Hand(Playable), "ドロー");
        return true;
    }



    #region Dragging
    private Vector3 previousPos;

    //プレイ位置をFieldから受け取る
    public int PlayPos { get; set; }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        //プレイできない際は、ドラッグできない
        if (!IsPlayable) return;

        //ドラッグ中のオブジェクトが例キャストをブロックしないように設定
        CanvasGroup.blocksRaycasts = false;

        //ドラッグ前の位置を記憶
        previousPos = Trn.parent.position;

        //PlayPosの値は、ドラッグ開始時には-1にしておく
        //EndDragのタイミングで-1出なかったら、プレイ位置が決定したことになる
        PlayPos = -1;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        //プレイできない際は、ドラッグできない
        if (!IsPlayable) return;

        //ドラッグ中のオブジェクトがポインターを追従するようにする
        Trn.parent.position = eventData.position;
    }

    public override IEnumerator OnEndDrag(PointerEventData eventData)
    {
        //プレイできない際は、ドラッグできない
        if (!IsPlayable) yield break;

        //レイキャストをブロックするように、設定を戻す
        CanvasGroup.blocksRaycasts = true;

        //プレイ位置が決まっており、かつマナを消費できたら(ここで消費はしない)、カードをプレイ
        if (PlayPos != -1 && Hand.I(IsPlayer).RemainedMana >= Playable.GetCost())
        {

            PlayableId playingCard = Trn.GetComponentInParent<IPlayable>().PlayableId;


            if (Playable.OnPlayAbility!=null && Playable.OnPlayAbility.TargetRequired )
            {
                yield return new TargetSelector().StartSelection((StatusBase)Playable, Playable.OnPlayAbility.TargetType, Playable.OnPlayAbility.Condition, target => OnTargetSelected(playingCard, target));
            }
            else
            {
                OnTargetSelected(playingCard, null);
            }

        }
        else
        {
            //位置を戻す
            Trn.parent.position = previousPos;
        }
    }
    #endregion


    /// <summary>
    /// スキルの対象として選択されたときに呼ばれる
    /// </summary>
    /// <param name="playingCard"></param>
    /// <param name="target"></param>
    private void OnTargetSelected(PlayableId playingCard, StatusBase target)
    {
        //コマンドを作成
        Command.Command command = target switch
        {

            //アビリティーターゲットとしてヒーローが選択された場合
            Status_Hero hero => new Command.Command_CardPlay(IsPlayer, playingCard, PlayPos, target.IsPlayer),

            //アビリティーターゲットとしてヒーローが選択された場合
            IPlayable playable => new Command.Command_CardPlay(IsPlayer, playingCard, PlayPos, playable.PlayableId, target.IsPlayer),

            //アビリティーターゲットが選択されていない場合
            _ => new Command.Command_CardPlay(IsPlayer, playingCard, PlayPos)
        };

        //コマンドを実行
        Command.CommandManager.I.Run(command);

        //ユーザーの入力の直前
        TurnManager.I.StartJudge(false);
    }


}
