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
        /// 実行されたコマンドを保持する
        /// </summary>
        private List<Command> commandHistory;

        public Subject<Command> ranCommand = new Subject<Command>();

        /// <summary>
        /// この端末上で生成されたコマンドを実行
        /// </summary>
        /// <param name="command"></param>
        public void Run(Command command)
        {
            //コマンドを実行
            command.Run();
            ranCommand.OnNext(command);
            //履歴に残す
            commandHistory.Add(command);
            //相手にコマンドを送信
            SendCommand(command);
        }

        /// <summary>
        /// 通信により受け取ったコマンドを実行
        /// </summary>
        /// <param name="command"></param>
        public void RPCRun(Command command)
        {
            //コマンドを実行
            command.RunPun();
            ranCommand.OnNext(command);
            //履歴に残す
            commandHistory.Add(command);
        }

        /// <summary>
        /// この端末上で生成されたコマンドを相手に送信する
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
        /// この端末上で生成されたコマンドとして実行されたときの処理
        /// </summary>
        internal abstract void Run();

        /// <summary>
        /// 通信で受け取ったコマンドとして実行された場合の処理
        /// </summary>
        internal abstract void RunPun();

        /// <summary>
        /// コマンドがうまく実行できたか否か
        /// </summary>
        public Result Result { get; protected set; }
        protected void SetResult(bool succeed) => Result = succeed ? Result.Succeed : Result.Failed;

        public abstract bool Same(Command command);
    }

}