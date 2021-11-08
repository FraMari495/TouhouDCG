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
    /// �J�[�h�Q�[���ɂ�����|�W�V����4��(Deck,Hand,Field,Discard)���p�����钊�ۃN���X
    /// �C���X�^���X�̐���2�Ɍ���(�V���O���g���I)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PositionBase<T> : MonoBehaviour, IPosition where T : PositionBase<T>
    {
        protected abstract PosEnum Pos { get; }
        //public Subject<Unit> CheckPlayable { get; private set; } = new Subject<Unit>();

        public Transform Transform => I(isPlayer).transform;


        [SerializeField] private bool isPlayer;

        private List<IPlayable> cards = new List<IPlayable>();

        protected void Shuffle()=> cards = cards.Shuffle().ToList();

        /// <summary>
        /// �v���C���[�̃|�W�V�������ۂ�
        /// </summary>
        public bool IsPlayer => isPlayer;

        /// <summary>
        /// 2�̃C���X�^���X
        /// </summary>
        private static Dictionary<bool, T> iMap;

        public List<Status_Chara> GetCardsOfRace(Race race)
            => Cards.ConvertType<Status_Chara>().NonNull().Where(c => c.CharaData.Race == race).ToList();

        /// <summary>
        /// ���̃|�W�V�����ɒu����Ă���J�[�h
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
                    Debug.LogError(typeof(T).ToString() + "�̃C���X�^���X��" + i.Length + "���݂��܂��B2�ɏC�����Ă�������");
                    return null;
                }
            }
            return iMap[isPlayer];

        }

        private void OnDestroy()
        {
            if (iMap != null)
            {
                iMap.ForEach(p => Destroy(p.Value.gameObject));
            }

            iMap = null;
        }

        protected virtual void Awake()
        {
            //�^�[���ύX�̒ʒm���󂯎��
            ReactivePropertyList.I.O_NewTurnNotif.Subscribe(turn =>
            {
                if (turn == IsPlayer) OnBeginTurn();
            });

            //�W���b�W�̃^�C�~���O�̒ʒm���󂯎��
            ReactivePropertyList.I.O_Judge.Subscribe(_ => Judge());
        }

        /// <summary>
        /// �s���s�\�ɂ��� (���[�U�[�̓��͂𖳌��ɂ���)
        /// </summary>
        public void Wait()
        {
            Cards.ForEach(x => x.IsPlayable = false);
        }

        /// <summary>
        /// �^�[���J�n���ɌĂ΂�郁�\�b�h
        /// </summary>
        protected virtual void OnBeginTurn() { }

        /// <summary>
        /// �W���b�W���ɌĂ΂�郁�\�b�h
        /// </summary>
        protected virtual void Judge() {}

        protected bool Move<U>(IPlayable playable,PositionBase<U> to,int posIndex,PosEnum? from_ = null)where U : PositionBase<U>
        {
            PosEnum from = from_ ?? Pos;

            //�f�[�^��Ńt�B�[���h�ɒǉ�
            if (to.AddCard(posIndex, playable))
            {
                //�|�W�V�����ύX��ʒm(�A�j���[�V����)
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
                        Debug.LogError("��菜���܂���ł���");
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
            //�|�W�V�����ύX��ʒm(�A�j���[�V����)
            spell.UpdatePosition.OnNext(( PosEnum.Hand, field.Pos, field.Cards.Count));
            if (Pos != field.Pos)
            {
                if (!cards.Remove(spell))
                {
                    Debug.LogError("��菜���܂���ł���");
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
            //cards�ɒǉ�
            cards.Insert(posIndex, playable);
            return true;
        }
    }



    public interface IPosition
    {
        ReadOnlyCollection<IPlayable> Cards { get; }
        Transform Transform { get; }
        void Wait();
    }

   
}
