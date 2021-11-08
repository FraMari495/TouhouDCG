using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// ステート
/// カードの入力受付の具体的な処理を担当
/// </summary>
public abstract class CardState
{

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="playable">関連付けたいIPlayable</param>
    public CardState(IPlayable playable)
    {
        Playable = playable;
        IsPlayer = playable.IsPlayer;
        Trn = playable.GameObject.transform;
    }

    /// <summary>
    /// カードのTransform
    /// </summary>
    protected Transform Trn { get; }

    public abstract void OnBeginDrag(PointerEventData eventData);
    public abstract void OnDrag(PointerEventData eventData);
    public abstract IEnumerator OnEndDrag(PointerEventData eventData);

    /// <summary>
    /// 行動(プレイ)可能か否か
    /// </summary>
    public bool IsPlayable => Playable.IsPlayable;

    /// <summary>
    /// 関連付けられているIPlayable
    /// </summary>
    public IPlayable Playable { get; }

    /// <summary>
    /// プレイヤーのカードか否か
    /// </summary>
    public  bool IsPlayer { get; }

    /// <summary>
    /// カードがこの状態に遷移したときの処理
    /// </summary>
    /// <returns移動に成功したか></returns>
    public abstract bool Enter();


    /// <summary>
    /// カードが表向きか否か
    /// </summary>
    public abstract bool Showing { get; }

    /// <summary>
    /// カードが置かれている場所
    /// </summary>
    public abstract PosEnum Pos { get; }
}
