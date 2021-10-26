using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

/// <summary>
/// ��Ԉُ�
/// </summary>
public enum StatusEffect
{
    Dead = 0,
    Freeze = 1,
    Sealed=2,
}

public class Status_Chara : Status, ICardViewInitializer, IPlayable
{
    /// <summary>
    /// �J�[�h�̏�����
    /// </summary>
    /// <param name="isPlayer">�v���C���[�̃J�[�h���ۂ�</param>
    /// <param name="cardBook">�J�[�h�̃f�[�^�x�[�X</param>
    public void Initialize(bool isPlayer, CardBook cardBook)
    {
        if (cardBook is CardBook_Chara charaBook)
        {
            CardBook = cardBook;
            IsPlayer = isPlayer;
            CardData = new Data_Chara(isPlayer, this.transform, charaBook, Dead);
            CardName = charaBook.CardName;
        }
        else
        {
            Debug.LogError("�L�����J�[�h��CardBook���K�v�ł�");
        }
    }

    #region UniRx.Subject

    /// <summary>
    /// �R�X�g�̕\����ύX����悤�ɒʒm�𑗂�
    /// </summary>
    public Subject<int> UpdateCostUI { get; } = new Subject<int>();

    /// <summary>
    /// Hp�̕\����ύX����悤�ɒʒm�𑗂�
    /// </summary>
    public Subject<int> UpdateAtkUI { get; } = new Subject<int>();

    /// <summary>
    /// �ʒu���ύX���ꂽ���Ƃ�ʒm(�A�j���[�V�������N��������)
    /// </summary>
    public Subject<(PosEnum from, PosEnum to, int index)> UpdatePosition { get; } = new Subject<(PosEnum from, PosEnum to, int index)>();

    public Subject<int> UpdateBombUI { get; } = new Subject<int>();
    #endregion


    #region �J�[�h�̃X�e�[�^�X public get

    /// <summary>
    /// �J�[�h�̃I�u�W�F�N�g
    /// </summary>
    public GameObject GameObject => this.gameObject;


    /// <summary>
    /// �U���͂�Hp�ȂǁA�l�X�ȏ�Ԃ������ϐ����܂܂�Ă���
    /// </summary>
    public Data_Chara CharaData => (Data_Chara)CardData;

    /// <summary>
    /// �J�[�h�̖��O
    /// </summary>
    public string CardName { get; private set; }

    /// <summary>
    /// �J�[�h�̃C���X�^���X��1��1�ɕR�Â�Id
    /// </summary>
    public PlayableId PlayableId => CharaData.PlayableId;

    /// <summary>
    /// �J�[�h�̎�ނ�1��1�ɕR�Â�Id
    /// </summary>
    public int CardBookId => CharaData.CardbookId;

    /// <summary>
    /// �v���C���ɔ�������A�r���e�B�[
    /// </summary>
    public OnPlayAbility OnPlayAbility => CharaData.OnPlaySkill;

    /// <summary>
    /// �J�[�h�̃f�[�^�x�[�X
    /// </summary>
    public CardBook CardBook { get; private set; }

    /// <summary>
    /// �J�[�h�̎��(�L�����J�[�h�Ȃ̂� Chara��Ԃ�)
    /// </summary>
    public override CardType Type => CardType.Chara;
    protected override string ObjectName => CardName;

    /// <summary>
    /// �U����
    /// </summary>
    public int Atk => CharaData.AtkData;

    /// <summary>
    /// �v���C�\���ۂ�
    /// </summary>
    public bool IsPlayable{ get => CharaData.playable; set => CharaData.playable = value; }

    #endregion


    #region public methods
    /// <summary>
    /// �J�[�h�̃C���X�^���XId���擾����
    /// </summary>
    /// <param name="playableId"></param>
    public void RequirePlayableId(int? playableId)
    {
        if (PlayableId != PlayableId.Default)
        {
            Debug.LogError("����ID���ݒ肳��Ă��܂�");
            return;
        }
        CharaData.PlayableId = PlayableIdManager.I.GetId(this, playableId);
    }
    /// <summary>
    /// (�ʏ�)�^�[���J�n���ɌĂ΂��
    /// �U���\�񐔂����Z�b�g����
    /// </summary>
    public void ResetAttackNum()=> CharaData.AttackNum = RemoveStatusEfect(StatusEffect.Freeze) ? 0 : CharaData.initialAttackNum;

    /// <summary>
    /// �^�[���I�����̃X�L���𔭓�
    /// </summary>
    public void RunOnTurnEndSkill()=> CharaData.MyAbilities.OnTurnEndSkill?.Run(this,null);

    /// <summary>
    /// ���S���̃X�L���𔭓�
    /// </summary>
    public void RunOnDefeatedSkill() => CharaData.MyAbilities.OnDefeatedSkill?.Run(this,null);

    /// <summary>
    /// �U���͂̌���
    /// </summary>
    /// <param name="delta"></param>
    public void DownAtk(int delta)
    {
        CharaData.AtkData.Down(delta);
        AnimationManager.I.AddSequence(() => AnimationMaker.AtkDownAnimation(delta, Atk, this),"�U���͌���");
    }

    /// <summary>
    /// �U���͂̑���
    /// </summary>
    /// <param name="delta"></param>
    public void AddAtk(int delta)
    {
        CharaData.AtkData.Add(delta);
        AnimationManager.I.AddSequence(() => DOTween.Sequence().AppendCallback(()=> UpdateAtkUI.OnNext(CharaData.AtkData)), "�U���͑���");
    }

    /// <summary>
    /// �R�X�g���擾
    /// </summary>
    /// <returns></returns>
    public int GetCost() => CharaData.Cost;

    /// <summary>
    /// ��Ԉُ��t�^
    /// </summary>
    /// <param name="effect"></param>
    public void AddEffect(StatusEffect effect)
    {
        CharaData.AddStatusEffect(effect);
        UpdateEffect.OnNext(effect);
        UpdateBombUI.OnNext(CharaData.MyAbilities.BombCost);
    }

    public int[] AddBomb(int addNum,int[] indices)
    {
        if (CharaData.MyAbilities.BombCost > 0)
        {
            bool run = CharaData.MyAbilities.AddBombCost(addNum);
            AnimationManager.I.AddSequence(() => DOTween.Sequence().AppendCallback(() => UpdateBombUI.OnNext(CharaData.MyAbilities.BombCost)), "�{������");

            if (run)
            {
                return CharaData.MyAbilities.BombAbility.Run(this, indices);
            }
            else
            {
                return null;
            }
        }

        return null;
    }

    #endregion

    protected override void Dead()=> AddEffect(StatusEffect.Dead);


    #region classes

    /// <summary>
    /// �L�����J�[�h�̏��
    /// </summary>
    public class Data_Chara:Data
    {
        /// <summary>
        /// �U����
        /// </summary>
        public Atk AtkData { get; set; }

        /// <summary>
        /// �R�X�g
        /// </summary>
        public Cost Cost { get; private set; }



        /// <summary>
        /// �J�[�h�̃C���X�^���X��Id
        /// (�ʐM�ΐ펞�ɃJ�[�h���w�肷��ۂɗp����)
        /// </summary>
        public PlayableId PlayableId { get; set; } = PlayableId.Default;

        /// <summary>
        /// ���ݔ������Ă������\��
        /// </summary>
        private RunningAbilities myAbilities;

        /// <summary>
        /// ���󂳂�Ă��Ȃ���� myAbilities ��Ԃ��A���󂳂�Ă���ꍇ�͋��RunningAbilities��Ԃ�
        /// </summary>
        public RunningAbilities MyAbilities => StatusEffects.Contains(StatusEffect.Sealed) ? new RunningAbilities(null) : myAbilities;

        /// <summary>
        /// 1�^�[���ɉ���U���ł��邩 (�ʏ��1��)
        /// </summary>
        public int initialAttackNum = 1;

        /// <summary>
        /// �c��̍U���\��
        /// </summary>
        public int AttackNum { get; set; }

        /// <summary>
        /// �s���\���ۂ�
        /// </summary>
        public bool playable { get; set; }
        public int CardbookId { get; }

        public Race Race { get; }

        public Data_Chara(bool isPlayer,Transform trn,CardBook_Chara charaBook,Action onDead)
            :base(isPlayer, trn, charaBook.Hp,charaBook.Hp, onDead)
        {
            CardbookId = charaBook.Id; //DB Id 
            AtkData = new Atk(charaBook.Atk);//�U���͂̏�����
            Cost = new Cost(charaBook.Cost);//�R�X�g�̏�����
            this.myAbilities = new RunningAbilities(charaBook);//����\�͂̏�����
            this.initialAttackNum = 1;//�U���\�񐔂�1(��X�����U���\�ȃJ�[�h�����Ƃ��͕ύX)
            AttackNum = 0;//���������^�[���͍U���s�\
            OnPlaySkill = charaBook.OnPlaySkill;//�v���C���X�L��(����̉e�����󂯂Ȃ��̂ŁA���̃N���X���Œ�`)
            Race = charaBook.Race;
        }

        /// <summary>
        /// AI���g�p����R���X�g���N�^
        /// �f�B�[�v�R�s�[
        /// </summary>
        /// <param name="charaData"></param>
        private Data_Chara(Data_Chara charaData):base(charaData.IsPlayer,null,charaData.HpData,charaData.HpData.Max,null)
        {
            CardbookId = charaData.CardbookId;
            AtkData = new Atk( charaData.AtkData);
            Cost = new Cost(charaData.Cost);
            this.myAbilities = RunningAbilities.AbiliyiesForAI(charaData.MyAbilities);
            this.initialAttackNum = charaData.initialAttackNum;
            AttackNum = 0;
            //   this.playable = playable;
            OnPlaySkill = charaData.OnPlaySkill;
            playable = charaData.playable;
            PlayableId = charaData.PlayableId;
            HpData.OnDead = () => StatusEffects.Add(StatusEffect.Dead);
            Race = charaData.Race;
        }

        /// <summary>
        /// �v���C���X�L��
        /// ����͕���̉e�����󂯂Ȃ����߂��̃N���X�ɋL�q
        /// </summary>
        public OnPlayAbility OnPlaySkill { get; private set; }

        /// <summary>
        /// AI�̎v�l�Ɏg�p
        /// charaData���f�B�[�v�R�s�[(�����Ƃ������@�͖����̂�)
        /// </summary>
        /// <param name="charaData"></param>
        /// <returns></returns>
        public static Data_Chara DataCharaForAI(Data_Chara charaData)
        {
            return new Data_Chara(charaData);
        }

        /// <summary>
        /// AI�̎v�l�Ɏg�p
        /// �J�[�h�̕]����Ԃ�
        /// </summary>
        /// <returns></returns>
        public float GetScore()
        {
            float ans = AtkData*3;
            ans += RunningAbilities.GetAbilityScore(this);
            return ans;
        }
    }

    /// <summary>
    /// �L�����J�[�h���������Ă������\��
    /// (����ɂ�������\��)
    /// </summary>
    public class RunningAbilities
    {
        public RunningAbilities(CardBook_Chara charaBook)
        {
            if (charaBook == null) //���󂳂�Ă���ꍇ
            {
                SpecialStatuses = new List<SpecialStatus>();
                BombCost = -1;
            }
            else  //����Ă��Ȃ��ꍇ
            {
                SpecialStatuses = charaBook.SpecialStatuses.ToList();
                OnDefeatedSkill = charaBook.OnDefeatedSkill;
                OnTurnEndSkill = charaBook.OnTurnEndSkill;
                BombAbility = charaBook.BombAbility;
                BombCost = charaBook.BombCost;
            }
        }

        /// <summary>
        /// AI�̎v�l�ɗp����
        /// ���f�B�[�v�R�s�[
        /// </summary>
        /// <param name="abilities"></param>
        private RunningAbilities(RunningAbilities abilities)
        {
            SpecialStatuses = abilities.SpecialStatuses;
            OnDefeatedSkill = abilities.OnDefeatedSkill;
            OnTurnEndSkill = abilities.OnTurnEndSkill;
            BombAbility = abilities.BombAbility;
            BombCost = abilities.BombCost;
        }

        /// <summary>
        /// ����X�e�[�^�X(�ђʁA�K�E�A���A�S�� = AP��ATK�̕\�����ω�����)
        /// </summary>
        public List<SpecialStatus> SpecialStatuses { get; private set; }

        /// <summary>
        /// ���S���X�L��
        /// </summary>
        public OnDefeatedAbility OnDefeatedSkill { get;�@private set; } = null;

        /// <summary>
        /// �^�[���I�����X�L��
        /// </summary>
        public OnTurnEndSkill OnTurnEndSkill { get; private set; } = null;

        public BombAbilityBase BombAbility { get; private set; } = null;

        private int bombCost = -1;
        public int BombCost
        {
            get => bombCost; 
            private set
            {
                bombCost = value;
            }
        }

        public bool AddBombCost(int cost)
        {
            if (BombCost > 0)
            {
                BombCost = Math.Max(BombCost - cost, 0);
                return BombCost == 0;
            }
            else return false;
        }


        /// <summary>
        /// �S�ǂ̐��l�́ASpecialStatuses���X�g�Ɋ܂܂��uSpecialStatus.Diffense�v�̐��Ō��肷��
        /// (������2��z��)
        /// </summary>
        public int Diffense => SpecialStatuses.Where(s => s == SpecialStatus.Diffense).Count();

        /// <summary>
        /// �����̓���X�e�[�^�X�������Ă��邩�ۂ�
        /// </summary>
        /// <param name="specialStatus"></param>
        /// <returns></returns>
        public bool HaveSpecialStatus(SpecialStatus specialStatus) => SpecialStatuses.Contains(specialStatus);
 
        /// <summary>
        /// AI�̎v�l�Ɏg�p
        /// ���̃C���X�^���X�����A�r���e�B�[�̍��v�X�R�A
        /// </summary>
        /// <param name="chara"></param>
        /// <returns></returns>
        public static float GetAbilityScore(Data_Chara chara)
        {
            RunningAbilities ability = chara.MyAbilities;
            float ans = -4;
            if (ability.SpecialStatuses.Contains(SpecialStatus.Gardian)) ans += chara.HpData*3+5;
            if (ability.SpecialStatuses.Contains(SpecialStatus.Killer)) ans += 6;
            if (ability.SpecialStatuses.Contains(SpecialStatus.Surprise)) ans += chara.AtkData;
            ans += ability.SpecialStatuses.Where(s => s == SpecialStatus.Diffense).Count() * chara.HpData / 2f;
            if (ability.SpecialStatuses.Contains(SpecialStatus.Freeze)) ans += 4;
            if (ability.SpecialStatuses.Contains(SpecialStatus.Penetrate)) ans += chara.AtkData;
            if (ability.OnDefeatedSkill) ans += 3;
            if (ability.OnTurnEndSkill) ans += 5;
            return ans;
        }

        /// <summary>
        /// AI�̎v�l�Ɏg�p
        /// ���̃C���X�^���X�����A�r���e�B�[�̍��v�X�R�A
        /// </summary>
        /// <param name="chara"></param>
        /// <returns></returns>
        public static float GetAbilityScore(Status_Chara chara)=> GetAbilityScore(chara.CharaData);

        /// <summary>
        /// AI�̎v�l(�V�~�����[�V����)�Ɏg�p
        /// ���̃C���X�^���X���f�B�[�v�R�s�[
        /// �����Ƒ��ɂ������@�������̂�?
        /// </summary>
        /// <param name="abilities"></param>
        /// <returns></returns>
        public static RunningAbilities AbiliyiesForAI(RunningAbilities abilities)=> new RunningAbilities(abilities);
    }
    #endregion

}
