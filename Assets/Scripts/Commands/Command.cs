using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Command
{

    public enum Result
    {
        InProgress = 0,
        Succeed = 1,
        Failed = 2
    }

    public class CommandManager
    {

        #region Singleton
        private static CommandManager instance = new CommandManager();
        public static CommandManager I => instance;
        #endregion
        private CommandManager() { commandHistory = new List<Command>(); }

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