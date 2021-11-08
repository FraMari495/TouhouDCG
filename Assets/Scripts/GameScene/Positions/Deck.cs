using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Position
{
    public class Deck : PositionBase<Deck>
    {
        protected override PosEnum Pos => PosEnum.Deck;

        //初期デッキ(図鑑idで設定)
        private int[] initialDeck;
        public int[] InitialDeck => initialDeck;

        #region public methods

        /// <summary>
        /// デッキを作成する
        /// </summary>
        /// <param name="initialDeck">図鑑Idにより表現されたデッキ</param>
        public void MakeDeck(int[] initialDeck)
        {
            //初期デッキを記憶
            this.initialDeck = initialDeck;

            //図鑑の準備
            CardBook[] allCards = Resources.LoadAll<CardBook>("CardBook");

            //図鑑Idからカードを生成
            foreach (var id in initialDeck)
            {
                //図鑑を見て、生成したいカードのページ(CardBook)を探す
                CardBook foundCard = Array.Find(allCards, x => x.Id == id);

                //デバッグ
                if (foundCard == null) Debug.LogError($"id = {id}のカードは存在しません");

                //カードのオブジェクトを生成し、デッキの子オブジェクトとする
                IPlayable playable = foundCard.MakeCardToDeck(IsPlayer, Cards.Count);
                //Deck.I(playable.IsPlayer).AddCard(cards.Count, playable);
                Move(playable, this, Cards.Count);
            }
        }

        /// <summary>
        /// 通信対戦相手のデッキを生成する
        /// </summary>
        /// <param name="initialDeck">図鑑Idにより表現された相手のデッキ</param>
        /// <param name="playableIds">カードのインスタンスId</param>
        public void MakeConnectedRivalDeck(int[] initialDeck,int[] playableIds)
        {
            //図鑑の準備
            CardBook[] allCards = Resources.LoadAll<CardBook>("CardBook");

            for (int i = 0; i < initialDeck.Length; i++)
            {
                int bookId = initialDeck[i];
                int playableId = playableIds[i];

                //図鑑を見て、生成したいカードのページを探す
                CardBook foundCard = Array.Find(allCards, x => x.Id == bookId);

                //デバッグ
                if (foundCard == null) Debug.LogError($"id = {bookId}のカードは存在しません");

                //カードのオブジェクトをデッキに生成
                var playable = foundCard.MakeCardToDeck(IsPlayer, Cards.Count, playableId);
                Move(playable, this, Cards.Count);
            }
        }

        /// <summary>
        /// 初期手札が決定した際に呼ばれる
        /// </summary>
        /// <param name="playables">初期手札</param>
        public void DecidedFiestHand(List<IPlayable> playables)
        {
            //再度デッキをシャッフルする
            //cards = Cards.OrderBy(i => Guid.NewGuid()).ToList();
            Shuffle();

            //playables 内の要素の順序と、表示されているカードの順序を一致させる
            playables.Sort((a,b)=>(int)a.GameObject.transform.position.x - (int)b.GameObject.transform.position.x);

            //選択した3枚を手札に加える
            playables.ForEach(card => Draw(card));

            //選択されなかったカードを元の位置に戻す
            for (int i = 0; i < Cards.Count; i++)
            {
                Cards[i].GameObject.transform.SetParent(this.transform,false);
                Cards[i].GameObject.transform.localPosition = Vector3.zero;
                Cards[i].GameObject.transform.localScale = new Vector3(1, 1, 1);
                Cards[i].GameObject.transform.SetSiblingIndex(i);
            }


            //以上の操作で自分のデッキに対するシャッフルが発生している
            //通信対戦相手にも、自分のデッキの順序を正しく伝える必要がある
            //よって、initialDeckの要素を正しく並べる操作を行う

            //initialDeckの初期化
            initialDeck = new int[initialDeck.Length];

            //デバッグ
            if (initialDeck.Length != playables.Count + Cards.Count) Debug.LogError("カード枚数が合いません");

            //初期手札として選択した3枚を　initialDeck[0..2]に配置
            for (int i = 0; i < playables.Count; i++)
            {
                initialDeck[i] = (int)playables[i].CardBookId;
            }

            // 初期手札以外をそれ以降に配置
            for (int i = 0; i < Cards.Count; i++)
            {
                initialDeck[i + 3] = (int)Cards[i].CardBookId;
            }
        }

        public bool Draw(IPlayable card = null)
        {
            //デッキがない場合はドローせずにfalseを返す
            if (Cards.Count < 1) return false;

            //指定がない場合、ドローするカードは山札の一番上
            card ??= Cards[0];

            return Move(card, Hand.I(card.IsPlayer), Hand.I(IsPlayer).Cards.Count);
        }

        /// <summary>
        /// ターン開始時にはカードをドローする
        /// </summary>
        protected override void OnBeginTurn() => Draw();

        #endregion

    }

}
