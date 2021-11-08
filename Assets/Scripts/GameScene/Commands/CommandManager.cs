using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Position;
using System.Collections.ObjectModel;
using System.Linq;
using System;

namespace Command
{

    public enum Result
    {
        InProgress = 0,
        Succeed = 1,
        Failed = 2
    }

    public class CommandManager : MonoSingleton<CommandManager>
    {

        private void Start()
        {
            commandHistory = new List<Command>();
            var PlayerPos = new Dictionary<PosEnum, IPosition>()
            {
                {PosEnum.Deck,Deck.I(true) },
                {PosEnum.Hand,Hand.I(true) },
                {PosEnum.Field,Field.I(true) },
                {PosEnum.Discard,Discard.I(true) },
            };

            var RivalPos = new Dictionary<PosEnum, IPosition>()
            {
                {PosEnum.Deck,Deck.I(false) },
                {PosEnum.Hand,Hand.I(false) },
                {PosEnum.Field,Field.I(false) },
                {PosEnum.Discard,Discard.I(false) },
            };

            PositionMap = new Dictionary<bool, Dictionary<PosEnum, IPosition>>()
            {
                { true, PlayerPos },
                { false, RivalPos }
            };
        }

        private Dictionary<bool,Dictionary<PosEnum, IPosition>> PositionMap { get; set; }

        public ReadOnlyCollection<IPlayable> GetCards(bool isPlayer,PosEnum pos)
        {
            return PositionMap[isPlayer][pos].Cards;
        }

        public int GetMana(bool isPlayer)
        {
            return ((Hand)PositionMap[isPlayer][PosEnum.Hand]).RemainedMana;
        }

        public void Wait()
        {
            PositionMap[true].ForEach(p => p.Value.Wait());
            PositionMap[false].ForEach(p => p.Value.Wait());
        }

        public void OnTurnEnd(bool endTurn)
        {
            Field.I(endTurn).OnTurnEnd();
        }

        public bool ExistGardian(bool onPlayerField)
        {
            return GetCards(onPlayerField, PosEnum.Field).ConvertType<Status_Chara>().NonNull().Any(c => c.CharaData.MyAbilities.HaveSpecialStatus(SpecialStatus.Gardian));
        }


        public bool CanBeTarget(bool onPlayerField,StatusBase statusBase)
        {
            bool existGardian = ExistGardian(onPlayerField);

            if(statusBase is Status_Chara chara)
            {
                bool survived = chara.Hp > 0;
                bool gardian = chara.CharaData.MyAbilities.HaveSpecialStatus(SpecialStatus.Gardian);
                bool exists = Field.I(onPlayerField).Cards.Contains((IPlayable)statusBase);
                return exists && survived && (gardian || !existGardian);
            }

            else if(statusBase is Status_Hero hero)
            {
                bool survived = hero.Hp > 0;
                return survived && !existGardian;
            }

            else
            {
                throw new NotImplementedException();
            }
        }

        public Transform GetPositionTransform(bool isPlayer,PosEnum pos)
        {
            return PositionMap[isPlayer][pos].Transform;
        }

        public void AttackForAISimulation(Status_Chara.Data_Chara attacker , Status_Chara.Data_Chara target)
        {
            Field.I(false).AttackForAISimulation(attacker, target);
        }



        /// <summary>
        /// ���s���ꂽ�R�}���h��ێ�����
        /// </summary>
        private List<Command> commandHistory;

        public Subject<Command> ranCommand = new Subject<Command>();

        /// <summary>
        /// ���̒[����Ő������ꂽ�R�}���h�����s
        /// </summary>
        /// <param name="command"></param>
        public void Run(Command command)
        {
            //�R�}���h�����s
            command.Run();
            ranCommand.OnNext(command);
            //�����Ɏc��
            commandHistory.Add(command);
            //����ɃR�}���h�𑗐M
            SendCommand(command);
        }

        /// <summary>
        /// �ʐM�ɂ��󂯎�����R�}���h�����s
        /// </summary>
        /// <param name="command"></param>
        public void RPCRun(Command command)
        {
            //�R�}���h�����s
            command.RunPun();
            ranCommand.OnNext(command);
            //�����Ɏc��
            commandHistory.Add(command);
        }

        /// <summary>
        /// ���̒[����Ő������ꂽ�R�}���h�𑊎�ɑ��M����
        /// </summary>
        /// <param name="command"></param>
        private void SendCommand(Command command)
        {
            ConnectionManager.Instance.SendCommand(command);
        }

    }


    [System.Serializable]
    public abstract class Command:ScriptableObject
    {
        public Command(bool isPlayer)
        {
            this.isPlayer = isPlayer;
            Result = Result.InProgress;
        }

        [SerializeField]private bool isPlayer;
        public bool IsPlayer => isPlayer;

        /// <summary>
        /// ���̒[����Ő������ꂽ�R�}���h�Ƃ��Ď��s���ꂽ�Ƃ��̏���
        /// </summary>
        internal abstract void Run();

        /// <summary>
        /// �ʐM�Ŏ󂯎�����R�}���h�Ƃ��Ď��s���ꂽ�ꍇ�̏���
        /// </summary>
        internal abstract void RunPun();

        /// <summary>
        /// �R�}���h�����܂����s�ł������ۂ�
        /// </summary>
        public Result Result { get; protected set; }
        protected void SetResult(bool succeed) => Result = succeed ? Result.Succeed : Result.Failed;

        public abstract bool Same(Command command);
    }

}