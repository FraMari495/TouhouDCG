using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Linq;
using System.Collections.ObjectModel;

namespace Position
{

    /// <summary>
    /// カードゲームにおけるポジション4つ(Deck,Hand,Field,Discard)が継承する抽象クラス
    /// インスタンスの数を2つに限定(シングルトン的)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MonoPair<T> : MonoBehaviour where T : MonoPair<T>
    {
        protected abstract PosEnum Pos { get; }

        [SerializeField] private bool isPlayer;

        private List<IPlayable> cards = new List<IPlayable>();

        protected void Shuffle()=> cards = cards.Shuffle().ToList();

        /// <summary>
        /// プレイヤーのポジションか否か
        /// </summary>
        public bool IsPlayer => isPlayer;

        /// <summary>
        /// 2つのインスタンス
        /// </summary>
        private static Dictionary<bool, T> iMap;

        public List<Status_Chara> GetCardsOfRace(Race race)
            => Cards.ConvertType<Status_Chara>().NonNull().Where(c => c.CharaData.Race == race).ToList();

        /// <summary>
        /// このポジションに置かれているカード
        /// </summary>
        public ReadOnlyCollection<IPlayable> Cards => cards.AsReadOnly();

        public static T I(bool isPlayer)
        {
            if (iMap == null)
            {
                T[] i = FindObjectsOfType<T>();
                if (i.Length == 2)
                {
                    iMap = new Dictionary<bool, T>()
                { { i[0].isPlayer, i[0] }, { i[1].isPlayer, i[1] } };
                }
                else
                {
                    Debug.LogError(typeof(T).ToString() + "のインスタンスが" + i.Length + "個存在します。2個に修正してください");
                    return null;
                }
            }
            return iMap[isPlayer];

        }

        private void OnDestroy()
        {
            iMap = null;
        }

        protected virtual void Awake()
        {
            Debug.Log(this.GetType());


            //ターン変更の通知を受け取る
            TurnManager.I.NewTurnNotif.Subscribe(turn =>
            {
                if (turn == IsPlayer) OnBeginTurn();
            });

            //ジャッジのタイミングの通知を受け取る
            TurnManager.I.Judge.Subscribe(_ => Judge());
        }

        /// <summary>
        /// 行動不能にする (ユーザーの入力を無効にする)
        /// </summary>
        public static void Wait()
        {
            List<IPlayable> allFieldCards = new List<IPlayable>();
            allFieldCards.AddRange(I(true).Cards);
            allFieldCards.AddRange(I(false).Cards);

            allFieldCards.ForEach(x => x.IsPlayable = false);
        }

        /// <summary>
        /// ターン開始時に呼ばれるメソッド
        /// </summary>
        protected virtual void OnBeginTurn() { }

        /// <summary>
        /// ジャッジ時に呼ばれるメソッド
        /// </summary>
        protected virtual void Judge() {}

        protected bool Move<U>(IPlayable playable,MonoPair<U> to,int posIndex,PosEnum? from_ = null)where U : MonoPair<U>
        {
            PosEnum from = from_ ?? Pos;

            //データ上でフィールドに追加
            if (to.AddCard(posIndex, playable))
            {
                //ポジション変更を通知(アニメーション)
                playable.UpdatePosition.OnNext((from, to.Pos, posIndex));
                if (from != to.Pos)
                {
                    var removeFrom = from switch
                    {
                        PosEnum.Deck => Deck.I(playable.IsPlayer).cards,
                        PosEnum.Hand => Hand.I(playable.IsPlayer).cards,
                        PosEnum.Field => Field.I(playable.IsPlayer).cards,
                        PosEnum.Discard => Discard.I(playable.IsPlayer).cards,
                        _ => throw new NotImplementedException()
                    };
                    if (!removeFrom.Remove(playable))
                    {
                        Debug.LogError("取り除けませんでした");
                    }
                }


                if(to.Pos == PosEnum.Field&&playable is Status_Spell)
                {
                    Move(playable, Discard.I(playable.IsPlayer), Discard.I(playable.IsPlayer).Cards.Count, PosEnum.Field);
                }



                return true;
            }

            return false;
        }

        protected bool MoveSpellForUsing(Status_Spell spell,Field field)
        {
            field.AddCard(field.Cards.Count, spell);
            //ポジション変更を通知(アニメーション)
            spell.UpdatePosition.OnNext(( PosEnum.Hand, field.Pos, field.Cards.Count));
            if (Pos != field.Pos)
            {
                if (!cards.Remove(spell))
                {
                    Debug.LogError("取り除けませんでした");
                }
            }


            if (field.Pos == PosEnum.Field && spell is Status_Spell)
            {
                Move(spell, Discard.I(spell.IsPlayer), Discard.I(spell.IsPlayer).Cards.Count, PosEnum.Field);
            }
            return true;
        }

        protected virtual bool AddCard(int posIndex, IPlayable playable)
        {
            //cardsに追加
            cards.Insert(posIndex, playable);
            return true;
        }
    }


}
