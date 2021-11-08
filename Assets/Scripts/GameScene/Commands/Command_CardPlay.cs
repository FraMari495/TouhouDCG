
using Position;
using UnityEngine;

namespace Command
{
    /// <summary>
    /// �J�[�h���v���C����R�}���h
    /// </summary>
    [System.Serializable, CreateAssetMenu(menuName = "Command/CardPlay")]
    public class Command_CardPlay : Command
    {
        #region �R���X�g���N�^
        /// <summary>
        /// �J�[�h���Ώۂ̃A�r���e�B�[�����̃J�[�h���v���C
        /// </summary>
        /// <param name="isPlayer">�v���C���[�̃R�}���h���ۂ�</param>
        /// <param name="playingCard">�v���C����J�[�h</param>
        /// <param name="position">�v���C�ʒu</param>
        /// <param name="target">�A�r���e�B�[�̃^�[�Q�b�g</param>
        /// <param name="isPlayerTarget"></param>
        public Command_CardPlay(bool isPlayer, int playingCard, int position, int target, bool isPlayerTarget):base(isPlayer)
        {
            this.playingCard = playingCard;
            this.position = position;
            this.skillTarget = target;
        }

        /// <summary>
        /// �q�[���[���Ώۂ̃A�r���e�B�[�����̃J�[�h���v���C
        /// </summary>
        /// <param name="isPlayer">�v���C���[�̃R�}���h���ۂ�</param>
        /// <param name="playingCard">�v���C����J�[�h</param>
        /// <param name="position">�v���C�ʒu</param>
        /// <param name="heroTarget">�ǂ���̃q�[���[���^�[�Q�b�g�Ȃ̂�</param>
        public Command_CardPlay(bool isPlayer, int playingCard, int position, bool heroTarget) :base(isPlayer)
        {
            this.playingCard = playingCard;
            this.position = position;
            this.targetingHero = heroTarget;
        }

        /// <summary>
        /// �^�[�Q�b�g��K�v�Ƃ��Ȃ��A�r���e�B�[����(�������̓A�r���e�B�[�̂Ȃ�)�J�[�h���v���C
        /// </summary>
        /// <param name="isPlayer">�v���C���[�̃R�}���h���ۂ�</param>
        /// <param name="playingCard">�v���C����J�[�h</param>
        /// <param name="position">�v���C�ʒu</param>
        public Command_CardPlay(bool isPlayer, int playingCard, int position) :base(isPlayer)
        {
            this.playingCard = playingCard;
            this.position = position;
            noTarget = true;
        }

        public Command_CardPlay(bool isPlayer, int playingCard, int position, StatusBase target) :base(isPlayer)
        {
            this.playingCard = playingCard;
            this.position = position;

            if (target == null)
            {
                noTarget = true;
            }
            else if (target is Status_Chara chara)
            {
                skillTarget = chara.PlayableCardId;
            }
            else if (target is Status_Hero hero)
            {
                targetingHero = hero.IsPlayer;
            }
            else
            {
                Debug.LogError(target.GetType() + "�̓^�[�Q�b�g�ɂł��܂���");
            }
        }
        #endregion

        #region Serializable Field
        [SerializeField] private int playingCard;
        [SerializeField] private int position;
        [SerializeField] private int skillTarget = (int)PlayableId.Default;
        [SerializeField] private bool targetingHero = false;
        [SerializeField] private bool isPlayerTarget = false;
        //  [SerializeField] private Command_SelectAbilityTarget commandSelecting;
        [SerializeField] private bool noTarget;
        [SerializeField] private int[] indices;
        #endregion

        #region Property (public get)
        public bool IsPlayerTarget => isPlayerTarget;
        public bool NoTarget => noTarget;
        public int PlayingCard => playingCard;
        public int Position => position;
        public int SkillTarget => skillTarget;
        public bool TargetingHero => targetingHero;

        #endregion


        internal void SetIndices(int[] indices)
        {
            this.indices = indices;
        }

        internal override void Run()
        {
            bool succeed = false;
            int[] decidedIndex = null;
            if (noTarget) //�^�[�Q�b�g�Ȃ�
            {
                (succeed, decidedIndex) = Hand.I(IsPlayer).PlayCard(Position, playingCard, null, false,null);
            }
            else if (SkillTarget == (int)PlayableId.Default) //PlayableId������ = �q�[���[���^�[�Q�b�g
            {
                (succeed, decidedIndex) = Hand.I(IsPlayer).PlayCard(Position, playingCard, targetingHero, targetingHero, null);
            }
            else //�J�[�h���^�[�Q�b�g
            {
                (succeed, decidedIndex) = Hand.I(IsPlayer).PlayCard(Position, playingCard, SkillTarget, IsPlayerTarget, null);
            }

            indices = decidedIndex;
            SetResult(succeed);
        }

        internal override void RunPun()
        {
            if (noTarget)//�^�[�Q�b�g�Ȃ�
            {
                Hand.I(!IsPlayer).PlayCard(Position, playingCard, null, false, indices);
            }
            else if (SkillTarget == (int)PlayableId.Default)//PlayableId������ = �q�[���[���^�[�Q�b�g
            {
                Hand.I(!IsPlayer).PlayCard(Position, playingCard, !targetingHero, !targetingHero, indices);
            }
            else //�J�[�h���^�[�Q�b�g
            {
                Hand.I(!IsPlayer).PlayCard(Position, playingCard, SkillTarget, IsPlayerTarget, indices);

            }
        }

        public override bool Same(Command command)
        {
            if (command is Command_CardPlay command_CardPlay)
            {
                return
                IsPlayerTarget == command_CardPlay.IsPlayerTarget
                  && NoTarget == command_CardPlay.NoTarget
                  && PlayingCard == command_CardPlay.playingCard
                  && TargetingHero == command_CardPlay.targetingHero;
            }
            else
            {
                return false;
            }
    }
    }
}
