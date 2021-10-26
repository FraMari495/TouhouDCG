using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

    /// <summary>
    /// �t�B�[���h��̂ǂ̃J�[�h���I�����ꂤ�邩
    /// </summary>
    public enum TargetEnum
    {
        All,
        Owner,

        OwnCards,
        Opponents,

        OwnRandom,
        OpponentRandom,

        NoTarget,
    }

    /// <summary>
    /// �s����
    /// </summary>
    public enum ConditionOperation
    {
        NoCondition,
        Equal,
        LessOrEqual,
        MoreOrEqual
    }

    /// <summary>
    /// �ǂ̃p�����[�^�[���Q�Ƃ��邩
    /// </summary>
    public enum Parameter
    {
        Cost,
        Hp,
        Atk
    }


/// <summary>
/// �A�r���e�B�[�̑ΏۂƂȂ邽�߂̏�����\���ł���
/// e.g. Atk��2�ȉ��̃J�[�h�A�R�X�g��4�ȏ�̃J�[�h
/// </summary>
[Serializable]
public class Condition
{
    public Condition(TargetEnum target, ConditionOperation conditionOperation, int threshold, Parameter targetParameter)
    {
        this.target = target;
        this.conditionOperation = conditionOperation;
        this.threshold = threshold;
        this.targetParameter = targetParameter;
    }

    #region ������\������p�����[�^�[
    [SerializeField] private TargetEnum target;
    [SerializeField] private ConditionOperation conditionOperation;
    [SerializeField] private int threshold;
    [SerializeField] private Parameter targetParameter;
    [SerializeField] private bool attackHero;

    /// <summary>
    /// �J�[�h�̏��L�҂Ɋւ��鐧��
    /// </summary>
    public TargetEnum Target => target;

    /// <summary>
    /// �X�e�[�^�X�Ɋւ��鐧����\�����鉉�Z�q
    /// e.g. Atk��2 "�ȉ�"
    /// </summary>
    public ConditionOperation ConditionOperation => conditionOperation;

    /// <summary>
    /// �ǂ̃X�e�[�^�X�Ɋ֐������������邩 
    ///  e.g. "Atk" ��2�ȉ�
    /// </summary>
    public Parameter TargetParameter => targetParameter;

    /// <summary>
    /// �X�e�[�^�X�Ɋւ��鐧����threshold
    /// e.g. Atk�� "2" �ȉ�
    /// </summary>
    public int Threshold => threshold;

    /// <summary>
    /// �q�[���[��I�����邱�Ƃ������邩
    /// (������)
    /// </summary>
    public bool AttackHero => attackHero;
    #endregion

    /// <summary>
    /// �����̃J�[�h�̒�����A�������N���A���Ă���J�[�h��T��
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="playerCards"></param>
    /// <param name="rivalCards"></param>
    /// <returns></returns>
    public List<StatusBase> DecideOption(StatusBase owner, List<StatusBase> playerCards, List<StatusBase> rivalCards)
    {
        //�A�r���e�B�[�g�p�҂̃J�[�h
        List<StatusBase> ownCards = owner.IsPlayer ? playerCards : rivalCards;

        //�A�r���e�B�[�g�p�҂ɂƂ��āA����̃J�[�h
        List<StatusBase> opponentCards = owner.IsPlayer ? rivalCards : playerCards;

        //�܂��̓J�[�h�̏��L�҂ōi�����ۂɐ����c��I����
        List<StatusBase> targetOption = null;

        //�J�[�h�̏��L�҂ōi��(e.g. ����̃J�[�h, �����_���Ȏ����̃J�[�h)
        switch (Target)
        {
            case TargetEnum.All:
                var list = new List<StatusBase>(playerCards);
                list.AddRange(rivalCards);
                targetOption = list;
                break;
            case TargetEnum.Owner:
                targetOption = new List<StatusBase>() { owner };
                break;
            case TargetEnum.OwnCards:
                targetOption = ownCards;
                break;
            case TargetEnum.Opponents:
                targetOption = opponentCards;
                break;
            case TargetEnum.OwnRandom:
                StatusBase a = ownCards[UnityEngine.Random.Range(0, ownCards.Count)];
                targetOption = new List<StatusBase>() { a };
                break;
            case TargetEnum.OpponentRandom:
                StatusBase op = opponentCards[UnityEngine.Random.Range(0, opponentCards.Count)];
                targetOption = new List<StatusBase>() { op };
                break;
            default:
                Debug.LogError("���̑Ώۂ͖������ł�");
                break;
        }


        //���L�҂ōi�����ۂɐ����c��I�������A����ɍi�荞��

        //�X�e�[�^�X�ōi�����ۂɐ����c��I����
        IEnumerable<StatusBase> ans = new List<StatusBase>();

        //���Z�q���f���Q�[�g�^�ŕ\������ (e.g. Atk��2"�ȉ�")
        Comparison<int> comparison = ConditionOperation switch
        {
            ConditionOperation.NoCondition => (int val, int threshold) => 1,
            ConditionOperation.Equal => (int val, int threshold) => val == threshold ? 1 : -1,
            ConditionOperation.LessOrEqual => (int val, int threshold) => val <= threshold ? 1 : -1,
            ConditionOperation.MoreOrEqual => (int val, int threshold) => val >= threshold ? 1 : -1,
            _ => throw new NotImplementedException()
        };

        //�X�e�[�^�X�̏����𖞂����I������T��
        ans = TargetParameter switch
        {
            Parameter.Cost => targetOption.ConvertType<IPlayable>().Where(c => comparison(c.GetCost(), Threshold) > 0).ConvertType<StatusBase>(),
            Parameter.Atk => targetOption.ConvertType<Status_Chara>().Where(c => comparison(c.Atk, Threshold) > 0),
            Parameter.Hp => targetOption.ConvertType<Status>().Where(c => comparison(c.Hp, Threshold) > 0),
            _ => throw new NotImplementedException()
        };


        return ans.ToList();
    }
}



