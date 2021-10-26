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