
using Position;
using UnityEngine;

namespace Command
{
    /// <summary>
    /// カードをプレイするコマンド
    /// </summary>
    [System.Serializable, CreateAssetMenu(menuName = "Command/CardPlay")]
    public class Command_CardPlay : Command
    {
        #region コンストラクタ
        /// <summary>
        /// カードが対象のアビリティー持ちのカードをプレイ
        /// </summary>
        /// <param name="isPlayer">プレイヤーのコマンドか否か</param>
        /// <param name="playingCard">プレイするカード</param>
        /// <param name="position">プレイ位置</param>
        /// <param name="target">アビリティーのターゲット</param>
        /// <param name="isPlayerTarget"></param>
        public Command_CardPlay(bool isPlayer, int playingCard, int position, int target, bool isPlayerTarget):base(isPlayer)
        {
            this.playingCard = playingCard;
            this.position = position;
            this.skillTarget = target;
        }

        /// <summary>
        /// ヒーローが対象のアビリティー持ちのカードをプレイ
        /// </summary>
        /// <param name="isPlayer">プレイヤーのコマンドか否か</param>
        /// <param name="playingCard">プレイするカード</param>
        /// <param name="position">プレイ位置</param>
        /// <param name="heroTarget">どちらのヒーローがターゲットなのか</param>
        public Command_CardPlay(bool isPlayer, int playingCard, int position, bool heroTarget) :base(isPlayer)
        {
            this.playingCard = playingCard;
            this.position = position;
            this.targetingHero = heroTarget;
        }

        /// <summary>
        /// ターゲットを必要としないアビリティー持ち(もしくはアビリティーのない)カードをプレイ
        /// </summary>
        /// <param name="isPlayer">プレイヤーのコマンドか否か</param>
        /// <param name="playingCard">プレイするカード</param>
        /// <param name="position">プレイ位置</param>
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
                Debug.LogError(target.GetType() + "はターゲットにできません");
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
            if (noTarget) //ターゲットなし
            {
                (succeed, decidedIndex) = Hand.I(IsPlayer).PlayCard(Position, playingCard, null, false,null);
            }
            else if (SkillTarget == (int)PlayableId.Default) //PlayableIdが無効 = ヒーローがターゲット
            {
                (succeed, decidedIndex) = Hand.I(IsPlayer).PlayCard(Position, playingCard, targetingHero, targetingHero, null);
            }
            else //カードがターゲット
            {
                (succeed, decidedIndex) = Hand.I(IsPlayer).PlayCard(Position, playingCard, SkillTarget, IsPlayerTarget, null);
            }

            indices = decidedIndex;
            SetResult(succeed);
        }

        internal override void RunPun()
        {
            if (noTarget)//ターゲットなし
            {
                Hand.I(!IsPlayer).PlayCard(Position, playingCard, null, false, indices);
            }
            else if (SkillTarget == (int)PlayableId.Default)//PlayableIdが無効 = ヒーローがターゲット
            {
                Hand.I(!IsPlayer).PlayCard(Position, playingCard, !targetingHero, !targetingHero, indices);
            }
            else //カードがターゲット
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
