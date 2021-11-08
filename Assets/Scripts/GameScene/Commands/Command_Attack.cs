
using Position;
using UnityEngine;

namespace Command
{
    /// <summary>
    /// 攻撃するコマンド
    /// </summary>
    [System.Serializable,CreateAssetMenu(menuName = "Command/Attack")]
    public class Command_Attack : Command
    {
        #region コンストラクタ
        /// <summary>
        /// カードを攻撃
        /// </summary>
        /// <param name="isPlayer">プレイヤーのコマンドか否か</param>
        /// <param name="attacker">アタッカーのカード</param>
        /// <param name="target">攻撃対象</param>
        public Command_Attack(bool isPlayer, int attacker, int target):base(isPlayer)
        {
            this.attacker = attacker;
            this.target = target;
        }

        /// <summary>
        /// ヒーローを攻撃
        /// </summary>
        /// <param name="isPlayer">プレイヤーのコマンドか否か</param>
        /// <param name="attacker">アタッカーのカード</param>
        public Command_Attack(bool isPlayer, int attacker) : base(isPlayer)
        {
            this.attacker = attacker;
            this.targetingHero = true;
        }
        #endregion

        #region Serializable Fields
        [SerializeField] private int attacker;
        [SerializeField] private int target;
        [SerializeField] private bool targetingHero = false;
        #endregion

        #region properties (public get)
        public int Attacker => attacker;
        public int Target => target;
        public bool TargetingHero => targetingHero;
        #endregion

        internal override void Run()
        {
            bool succeed = false;
            if (targetingHero)//ヒーローをターゲット
            {
                succeed = Field.I(IsPlayer).AttackHero((int)attacker);
            }
            else//カードをターゲット
            {
                succeed = Field.I(IsPlayer).Attack((int)attacker, (int)target);
            }
            SetResult(succeed);
        }

        internal override void RunPun()
        {
            if (targetingHero)//ヒーローをターゲット
            {
                Field.I(!IsPlayer).AttackHero((int)attacker);
            }
            else//カードをターゲット
            {
                Field.I(!IsPlayer).Attack((int)attacker, (int)target);
            }
        }

        public override bool Same(Command command)
        {

            if (command is Command_Attack command_attack && IsPlayer == command.IsPlayer)
            {
                if (targetingHero)
                {
                    return Attacker == command_attack.Attacker
                        && TargetingHero == command_attack.TargetingHero;
                }
                else
                {
                    return Attacker == command_attack.Attacker
                        && Target == command_attack.Target
                        && TargetingHero == command_attack.TargetingHero;
                }
            }
            else
            {
                return false;
            }

        }
    }
}
