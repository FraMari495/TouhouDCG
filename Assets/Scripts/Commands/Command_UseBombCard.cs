
using Position;
using UnityEngine;

namespace Command
{
    /// <summary>
    /// ボムカードを使用するコマンド
    /// </summary>
    [System.Serializable,CreateAssetMenu(menuName = "Command/UseBombCard")]
    public class Command_UseBombCard : Command
    {
        [SerializeField] private int bombCardNumber;

        public Command_UseBombCard(int bombCardNumber, bool isPlayer):base(isPlayer)
        {
            this.bombCardNumber = bombCardNumber;
        }

        internal override void Run()
        {
            Hand.I(IsPlayer).GetBombCard(bombCardNumber);
        }

        internal override void RunPun()
        {
            Hand.I(!IsPlayer).GetBombCard(bombCardNumber);
        }

        public override bool Same(Command command)
        {
            return command is Command_UseBombCard;
        }
    }
}
