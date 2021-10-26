using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Player;
using UniRx;

namespace Position
{

    public class Hand : MonoPair<Hand>
    {
        [SerializeField] private TextMeshProUGUI manaText;
        [SerializeField] private Animator bombCardAnimator;


        /// <summary>
        /// 使用可能なマナ
        /// </summary>
        public int RemainedMana => Mana.RemainMana;

        /// <summary>
        /// 手札にカードを追加するメソッド
        /// 加えられたらtrueを返す
        /// </summary>
        /// <param name="playable">追加したいカード</param>
        /// <returns></returns>
        protected override bool AddCard(int _,IPlayable playable)
        {
            //手札には上限がある
            if (Cards.Count >= limit) return false;

            return base.AddCard(_,playable);
        }

        /// <summary>
        /// カードをプレイする
        /// </summary>
        /// <param name="posIndex">プレイする場所(index)</param>
        /// <param name="playingCardId">プレイするカードのインスタンスId</param>
        /// <param name="targetObj">アビリティーの対象</param>
        /// <param name="targetIsPlayer">ターゲットがプレイヤーのカードか否か</param>
        /// <returns></returns>
        public (bool succeed,int[] decidedIndices) PlayCard(int posIndex, PlayableId playingCardId,object targetObj,bool targetIsPlayer,int[] indices)
        {
            //プレイしたいカードを、cardIndexから特定する
            //(注)
            //status = cards[cardIndex]はNG
            //ユーザーが選択した物を特定数場合は、ゲームオブジェクトの配置から決定すべき
            IPlayable playable = PlayableIdManager.I.GetPlayableById(playingCardId);

            bool success = false;
            //カードを移動
            if (playable is Status_Chara)
            {
                success = Move(playable, Field.I(IsPlayer), posIndex);//playable switch
            }
            else if (playable is Status_Spell spell)
            {
                success = MoveSpellForUsing(spell, Field.I(IsPlayer));
            }
            //{
            //    Status_Chara c => Move(playable, Field.I(IsPlayer), posIndex),
            //    Status_Spell s => Move(playable, Discard.I(IsPlayer), posIndex),
            //    _ => throw new System.NotImplementedException()
            //};

            if (!success) return (false,null);

            //コストを支払う
            if (!Mana.UseMana(playable.GetCost())) throw new System.Exception("コストが支払えません");

            //アビリティーの対象
            StatusBase target = targetObj switch
            {
                PlayableId targetId => (StatusBase)(PlayableIdManager.I.GetPlayableById(targetId)),
                bool targetBool => Status_Hero.I(!IsPlayer),
                _ => null
            };

            int[] decidedIndex = playable.OnPlayAbility ?. Run((StatusBase)playable, target, indices);

            return (true,decidedIndex);
        }


        /// <summary>
        /// 手札の上限
        /// </summary>
        private int limit = 7;

        /// <summary>
        /// マナ
        /// </summary>
        private Mana Mana { get; set; }

        protected override PosEnum Pos => PosEnum.Hand;

        protected override void Awake()
        {
            base.Awake();
            Mana = new Mana(manaText);

        }

        protected override void Judge()
        {
            //手札のすべてのカードを調べ、プレイ可能かを判断する
            foreach (var card in Cards)
            {
                bool condition1 = TurnManager.I.Turn == IsPlayer;//自分のターンである
                bool condition2 = card.GetCost() <= Mana.RemainMana;//コストを支払える
                bool playableChara = (card is Status_Spell) || Cards.Count < 8;
                card.IsPlayable = condition1 && condition2;//以上の2つとも満たしている
            }
            Cards.ForEach(c => c.GameObject.GetComponent<CardInputHandler>().UpdatePlayableAura());

        }

        /// <summary>
        /// ターン開始時に呼ばれるメソッド
        /// </summary>
        protected override void OnBeginTurn()
        {
            Mana.NewTurn();
            bombCardAnimator.SetBool("Show", true);
        }

        public bool SpecialSummon(IPlayable playable)
        {
            int posInt = Cards.Count;

            //データ上で手札に追加
            if (AddCard(posInt, playable))
            {
                //ポジション変更を通知(アニメーション)
                playable.UpdatePosition.OnNext((PosEnum.None, Pos, posInt));
                return true;
            }

            return false;
        }

        public bool UseMana(int cost)=> Mana.UseMana(cost);

        [SerializeField] private CardBook_Spell bomb1;
        [SerializeField] private CardBook_Spell bomb2;
        public void GetBombCard(int bombCardNumber)
        {
            bombCardAnimator.SetBool("Show", false);
            var card = (bombCardNumber == 0 ? bomb1 : bomb2).MakeCardToHand(IsPlayer);
            card.GameObject.GetComponent<CanvasGroup>().alpha = 0;
            SpecialSummon(card);
        }
    }
}
