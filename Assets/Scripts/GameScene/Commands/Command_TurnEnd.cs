using UnityEngine;

namespace Command
{
    /// <summary>
    /// ターンを終了するコマンド
    /// </summary>
    [System.Serializable,CreateAssetMenu(menuName ="Command/TurnEnd")]
    public class Command_TurnEnd : Command
    {
        public Command_TurnEnd(bool isPlayer):base(isPlayer)
        {
        }

    [SerializeField] private int n = 1;



        public override bool Same(Command command)
        {
            if (command is Command_TurnEnd command_turnend)
            {
                return IsPlayer == command_turnend.IsPlayer;
            }
            else return false;
        }

        internal override void Run()
        {
            ReactivePropertyList.I.ChangeTurn();
        }

        internal override void RunPun()
        {

            //ターンを交代
            ReactivePropertyList.I.ChangeTurn();
        }
    }
}
