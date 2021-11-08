using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Position
{
    public class Deck : PositionBase<Deck>
    {
        protected override PosEnum Pos => PosEnum.Deck;

        //�����f�b�L(�}��id�Őݒ�)
        private int[] initialDeck;
        public int[] InitialDeck => initialDeck;

        #region public methods

        /// <summary>
        /// �f�b�L���쐬����
        /// </summary>
        /// <param name="initialDeck">�}��Id�ɂ��\�����ꂽ�f�b�L</param>
        public void MakeDeck(int[] initialDeck)
        {
            //�����f�b�L���L��
            this.initialDeck = initialDeck;

            //�}�ӂ̏���
            CardBook[] allCards = Resources.LoadAll<CardBook>("CardBook");

            //�}��Id����J�[�h�𐶐�
            foreach (var id in initialDeck)
            {
                //�}�ӂ����āA�����������J�[�h�̃y�[�W(CardBook)��T��
                CardBook foundCard = Array.Find(allCards, x => x.Id == id);

                //�f�o�b�O
                if (foundCard == null) Debug.LogError($"id = {id}�̃J�[�h�͑��݂��܂���");

                //�J�[�h�̃I�u�W�F�N�g�𐶐����A�f�b�L�̎q�I�u�W�F�N�g�Ƃ���
                IPlayable playable = foundCard.MakeCardToDeck(IsPlayer, Cards.Count);
                //Deck.I(playable.IsPlayer).AddCard(cards.Count, playable);
                Move(playable, this, Cards.Count);
            }
        }

        /// <summary>
        /// �ʐM�ΐ푊��̃f�b�L�𐶐�����
        /// </summary>
        /// <param name="initialDeck">�}��Id�ɂ��\�����ꂽ����̃f�b�L</param>
        /// <param name="playableIds">�J�[�h�̃C���X�^���XId</param>
        public void MakeConnectedRivalDeck(int[] initialDeck,int[] playableIds)
        {
            //�}�ӂ̏���
            CardBook[] allCards = Resources.LoadAll<CardBook>("CardBook");

            for (int i = 0; i < initialDeck.Length; i++)
            {
                int bookId = initialDeck[i];
                int playableId = playableIds[i];

                //�}�ӂ����āA�����������J�[�h�̃y�[�W��T��
                CardBook foundCard = Array.Find(allCards, x => x.Id == bookId);

                //�f�o�b�O
                if (foundCard == null) Debug.LogError($"id = {bookId}�̃J�[�h�͑��݂��܂���");

                //�J�[�h�̃I�u�W�F�N�g���f�b�L�ɐ���
                var playable = foundCard.MakeCardToDeck(IsPlayer, Cards.Count, playableId);
                Move(playable, this, Cards.Count);
            }
        }

        /// <summary>
        /// ������D�����肵���ۂɌĂ΂��
        /// </summary>
        /// <param name="playables">������D</param>
        public void DecidedFiestHand(List<IPlayable> playables)
        {
            //�ēx�f�b�L���V���b�t������
            //cards = Cards.OrderBy(i => Guid.NewGuid()).ToList();
            Shuffle();

            //playables ���̗v�f�̏����ƁA�\������Ă���J�[�h�̏�������v������
            playables.Sort((a,b)=>(int)a.GameObject.transform.position.x - (int)b.GameObject.transform.position.x);

            //�I������3������D�ɉ�����
            playables.ForEach(card => Draw(card));

            //�I������Ȃ������J�[�h�����̈ʒu�ɖ߂�
            for (int i = 0; i < Cards.Count; i++)
            {
                Cards[i].GameObject.transform.SetParent(this.transform,false);
                Cards[i].GameObject.transform.localPosition = Vector3.zero;
                Cards[i].GameObject.transform.localScale = new Vector3(1, 1, 1);
                Cards[i].GameObject.transform.SetSiblingIndex(i);
            }


            //�ȏ�̑���Ŏ����̃f�b�L�ɑ΂���V���b�t�����������Ă���
            //�ʐM�ΐ푊��ɂ��A�����̃f�b�L�̏����𐳂����`����K�v������
            //����āAinitialDeck�̗v�f�𐳂������ׂ鑀����s��

            //initialDeck�̏�����
            initialDeck = new int[initialDeck.Length];

            //�f�o�b�O
            if (initialDeck.Length != playables.Count + Cards.Count) Debug.LogError("�J�[�h�����������܂���");

            //������D�Ƃ��đI������3�����@initialDeck[0..2]�ɔz�u
            for (int i = 0; i < playables.Count; i++)
            {
                initialDeck[i] = (int)playables[i].CardBookId;
            }

            // ������D�ȊO������ȍ~�ɔz�u
            for (int i = 0; i < Cards.Count; i++)
            {
                initialDeck[i + 3] = (int)Cards[i].CardBookId;
            }
        }

        public bool Draw(IPlayable card = null)
        {
            //�f�b�L���Ȃ��ꍇ�̓h���[������false��Ԃ�
            if (Cards.Count < 1) return false;

            //�w�肪�Ȃ��ꍇ�A�h���[����J�[�h�͎R�D�̈�ԏ�
            card ??= Cards[0];

            return Move(card, Hand.I(card.IsPlayer), Hand.I(IsPlayer).Cards.Count);
        }

        /// <summary>
        /// �^�[���J�n���ɂ̓J�[�h���h���[����
        /// </summary>
        protected override void OnBeginTurn() => Draw();

        #endregion

    }

}
