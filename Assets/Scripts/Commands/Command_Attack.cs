
using Position;
using UnityEngine;

namespace Command
{
    /// <summary>
    /// �U������R�}���h
    /// </summary>
    [System.Serializable,CreateAssetMenu(menuName = "Command/Attack")]
    public class Command_Attack : Command
    {
        #region �R���X�g���N�^
        /// <summary>
        /// �J�[�h���U��
        /// </summary>
        /// <param name="isPlayer">�v���C���[�̃R�}���h���ۂ�</param>
        /// <param name="attacker">�A�^�b�J�[�̃J�[�h</param>
        /// <param name="target">�U���Ώ�</param>
        public Command_Attack(bool isPlayer, PlayableId attacker, PlayableId target):base(isPlayer)
        {
            this.attacker = attacker;
            this.target = target;
        }

        /// <summary>
        /// �q�[���[���U��
        /// </summary>
        /// <param name="isPlayer">�v���C���[�̃R�}���h���ۂ�</param>
        /// <param name="attacker">�A�^�b�J�[�̃J�[�h</param>
        public Command_Attack(bool isPlayer, PlayableId attacker) : base(isPlayer)
        {
            this.attacker = attacker;
            this.targetingHero = true;
        }
        #endregion

        #region Serializable Fields
        [SerializeField] private PlayableId attacker;
        [SerializeField] private PlayableId target;
        [SerializeField] private bool targetingHero = false;
        #endregion

        #region properties (public get)
        public PlayableId Attacker => attacker;
        public PlayableId Target => target;
        public bool TargetingHero => targetingHero;
        #endregion

        internal override void Run()
        {
            bool succeed = false;
            if (targetingHero)//�q�[���[���^�[�Q�b�g
            {
                succeed = Field.I(IsPlayer).AttackHero(attacker);
            }
            else//�J�[�h���^�[�Q�b�g
            {
                succeed = Field.I(IsPlayer).Attack(attacker, target);
            }
            SetResult(succeed);
        }

        internal override void RunPun()
        {
            if (targetingHero)//�q�[���[���^�[�Q�b�g
            {
                Field.I(!IsPlayer).AttackHero(attacker);
            }
            else//�J�[�h���^�[�Q�b�g
            {
                Field.I(!IsPlayer).Attack(attacker, target);
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