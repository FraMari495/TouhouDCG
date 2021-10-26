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
        /// �g�p�\�ȃ}�i
        /// </summary>
        public int RemainedMana => Mana.RemainMana;

        /// <summary>
        /// ��D�ɃJ�[�h��ǉ����郁�\�b�h
        /// ������ꂽ��true��Ԃ�
        /// </summary>
        /// <param name="playable">�ǉ��������J�[�h</param>
        /// <returns></returns>
        protected override bool AddCard(int _,IPlayable playable)
        {
            //��D�ɂ͏��������
            if (Cards.Count >= limit) return false;

            return base.AddCard(_,playable);
        }

        /// <summary>
        /// �J�[�h���v���C����
        /// </summary>
        /// <param name="posIndex">�v���C����ꏊ(index)</param>
        /// <param name="playingCardId">�v���C����J�[�h�̃C���X�^���XId</param>
        /// <param name="targetObj">�A�r���e�B�[�̑Ώ�</param>
        /// <param name="targetIsPlayer">�^�[�Q�b�g���v���C���[�̃J�[�h���ۂ�</param>
        /// <returns></returns>
        public (bool succeed,int[] decidedIndices) PlayCard(int posIndex, PlayableId playingCardId,object targetObj,bool targetIsPlayer,int[] indices)
        {
            //�v���C�������J�[�h���AcardIndex������肷��
            //(��)
            //status = cards[cardIndex]��NG
            //���[�U�[���I������������萔�ꍇ�́A�Q�[���I�u�W�F�N�g�̔z�u���猈�肷�ׂ�
            IPlayable playable = PlayableIdManager.I.GetPlayableById(playingCardId);

            bool success = false;
            //�J�[�h���ړ�
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

            //�R�X�g���x����
            if (!Mana.UseMana(playable.GetCost())) throw new System.Exception("�R�X�g���x�����܂���");

            //�A�r���e�B�[�̑Ώ�
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
        /// ��D�̏��
        /// </summary>
        private int limit = 7;

        /// <summary>
        /// �}�i
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
            //��D�̂��ׂẴJ�[�h�𒲂ׁA�v���C�\���𔻒f����
            foreach (var card in Cards)
            {
                bool condition1 = TurnManager.I.Turn == IsPlayer;//�����̃^�[���ł���
                bool condition2 = card.GetCost() <= Mana.RemainMana;//�R�X�g���x������
                bool playableChara = (card is Status_Spell) || Cards.Count < 8;
                card.IsPlayable = condition1 && condition2;//�ȏ��2�Ƃ��������Ă���
            }
            Cards.ForEach(c => c.GameObject.GetComponent<CardInputHandler>().UpdatePlayableAura());

        }

        /// <summary>
        /// �^�[���J�n���ɌĂ΂�郁�\�b�h
        /// </summary>
        protected override void OnBeginTurn()
        {
            Mana.NewTurn();
            bombCardAnimator.SetBool("Show", true);
        }

        public bool SpecialSummon(IPlayable playable)
        {
            int posInt = Cards.Count;

            //�f�[�^��Ŏ�D�ɒǉ�
            if (AddCard(posInt, playable))
            {
                //�|�W�V�����ύX��ʒm(�A�j���[�V����)
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
