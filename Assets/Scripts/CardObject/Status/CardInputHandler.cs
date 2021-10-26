using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using System;
using Player.CardHandler;

/// <summary>
/// カードが存在しうるポジション
/// </summary>
public enum PosEnum
{
    Deck = 0,
    Hand = 1,
    Field = 2,
    Discard = 3,
    None = -1
}

/// <summary>
/// カードの種類
/// (ヒーローもカードの一種と考える)
/// </summary>
public enum CardType
{
    Chara,
    Spell,
    Hero
}

namespace Player
{
    /// <summary>
    /// カードに対するユーザーの入力を受け付ける機能
    /// </summary>
    public abstract class CardInputHandler : MonoBehaviour, ICardViewInitializer,IPointerClickHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
    {
        [SerializeField] private CardVisualController cardVisual;

        private void Start()
        {
            cardVisual = this.GetComponentInChildren<CardVisualController>();
        }

        private string CardName { get; set; }
        protected IPlayable playable;

        /// <summary>
        /// カードの位置
        /// </summary>
        public PosEnum Pos => CurrentState.Pos;

        /// <summary>
        /// カードが表向きか否か
        /// </summary>
        public bool Showing => CurrentState.Showing;

        /// <summary>
        /// プレイヤーのカードか否か
        /// </summary>
        public bool IsPlayer { get; private set; }

        /// <summary>
        /// ステート
        /// </summary>
        public CardState CurrentState { get; protected set; } 

        /// <summary>
        /// プレイ可能オーラの表示非表示
        /// </summary>
        public void UpdatePlayableAura()
        {
            cardVisual.ChangeUsable(Showing && playable.IsPlayable);
        }

        /// <summary>
        /// カードの表示を、データとしてのカードの位置に合わせる
        /// </summary>
        public void ChanegCardView(PosEnum pos)
        {
            cardVisual.ChangeObject(Showing? pos: PosEnum.Deck);
        }

        public override string ToString() => CardName;

        /// <summary>
        /// カードの初期化
        /// </summary>
        /// <param name="isPlayer"></param>
        /// <param name="cardBook"></param>
        public virtual void Initialize(bool isPlayer, CardBook cardBook)
        {
            IsPlayer = isPlayer;
            CardName = cardBook.CardName;
            playable = this.GetComponent<IPlayable>();
            CurrentState = null;//new NullState(playable);
            Subscribing();
        }

        private void Subscribing()
        {
            //カードの位置変化に関するイベントを受け取る
            playable.UpdatePosition.Subscribe(pos => {

                if (pos.from == PosEnum.None)
                {
                    switch (pos.to)
                    {
                        case PosEnum.Field:
                            if (this is CardInputHandler_Chara chara) chara.ToFieldSpecial(pos.index);
                            else throw new Exception(this + "はキャラカードではありません");
                            break;
                        case PosEnum.Hand:
                            ToHandSpecial();
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    return;
                }
                else
                {

                    switch (pos.to)
                    {
                        case PosEnum.Deck:
                            ToDeck(pos.index);
                            break;
                        case PosEnum.Hand:
                            ToHand();
                            break;
                        case PosEnum.Field:
                            ToField(pos.index);
                            //else throw new Exception(this + "はキャラカードではありません");
                            break;
                        case PosEnum.Discard:
                            ToDiscard(this is CardInputHandler_Chara);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            });
        }


        #region Event Handler
        //現在のステートに応じて機能が変化する
        public void OnBeginDrag(PointerEventData eventData)
        {
            CurrentState?.OnBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            CurrentState?.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if(CurrentState!=null)StartCoroutine(CurrentState.OnEndDrag(eventData));
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (CurrentState != null&&Showing)
            {
                CardExplanation.I.Initialize(cardVisual.GetExampleCard());
            }
        }

        #endregion

        #region カードの遷移
        /// <summary>
        /// カードをデッキに移動
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool ToDeck(int pos)
        {
         //   if (CurrentState != null) CurrentState.Exit();
            CurrentState = new CardState_Deck(playable, pos);
            return CurrentState.Enter();
        }

        /// <summary>
        /// カードを手札に移動
        /// </summary>
        /// <returns></returns>
        public abstract bool ToHand();

        /// <summary>
        /// カードを手札に召喚
        /// </summary>
        /// <returns></returns>
        public bool ToHandSpecial()
        {
            if (this is CardInputHandler_Chara chara)
            {
                CurrentState = new CardState_Hand_Chara(playable, true);
            }
            else if (this is CardInputHandler_Spell spell)
            {
                CurrentState = new CardState_Hand_Spell(playable, true);
            }
            return CurrentState.Enter();
        }

        /// <summary>
        /// カードをフィールドに移動
        /// </summary>
        /// <returns></returns>
        public abstract bool ToField(int pos);

        /// <summary>
        /// カードを捨て札に移動
        /// </summary>
        /// <returns></returns>
        public bool ToDiscard(bool defeated)
        {
           // if (CurrentState != null) CurrentState.Exit();
            CurrentState = new CardState_Discard(playable, defeated);
            return CurrentState.Enter();
        }
        #endregion
    }
}
