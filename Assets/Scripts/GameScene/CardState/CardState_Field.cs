using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CardState_Field : CardState
{
    /// <summary>
    /// プレイ位置(現在の位置ではないため注意)
    /// 間違いそうなので、privateとする
    /// </summary>
    protected int Index { get;private set; }

    protected bool Special { get; private set; }
    public CardState_Field(IPlayable playable,int index,bool special) : base(playable)
    {
        //プレイ位置
        Index = index;
        Special = special;
      //  if (!(playable is Status_Chara)) Debug.LogError("キャラカード以外がCardState_Hand_Charaステートに遷移できません");
    }




    public override bool Showing =>true;

    public override PosEnum Pos => PosEnum.Field;

    #region Dragging

    public int Target { get; set; }
    public bool AttackHero { get; set; }



    public override void OnBeginDrag(PointerEventData eventData)
    {
        //攻撃可能でないときは、ドラッグできない
        if (!IsPlayable) return;

        //矢印の表示
        ArrowController.CreateArrow(IsPlayer,eventData, Trn);

        //TargetPosの値は、ドラッグ開始時に-1とする
        //OnEndDragの時点で-1出なかった場合は、攻撃対象が決定したということ
        Target = (int)PlayableId.Default;

        //AttackHeroの値は、ドラッグ開始時にfalseとする
        //OnEndDragの時点でtrueだった場合は、攻撃対象がHeroとなったということ
        AttackHero = false;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        //攻撃可能でないときは、ドラッグできない
        if (!IsPlayable) return;
    }

    public override IEnumerator OnEndDrag(PointerEventData eventData)
    {
        //攻撃可能でないときは、ドラッグできない
        if (!IsPlayable) yield break;

        int attacker = Playable.PlayableCardId;
        //攻撃対象が敵カードだった場合
        if (Target != (int)PlayableId.Default)
        {
            Command.CommandManager.I.Run(new Command.Command_Attack(IsPlayer, attacker, Target));
        }

        //攻撃対象がHeroだった場合
        if (AttackHero)
        {
            Command.CommandManager.I.Run(new Command.Command_Attack(IsPlayer, attacker));
        }

        //ユーザーの入力の直前
        //TurnManager.I.StartJudge(false);
        ConnectionManager.Instance.Judge();
    }
    #endregion

}
