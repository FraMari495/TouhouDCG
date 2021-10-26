using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

    /// <summary>
    /// フィールド上のどのカードが選択されうるか
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
    /// 不等号
    /// </summary>
    public enum ConditionOperation
    {
        NoCondition,
        Equal,
        LessOrEqual,
        MoreOrEqual
    }

    /// <summary>
    /// どのパラメーターを参照するか
    /// </summary>
    public enum Parameter
    {
        Cost,
        Hp,
        Atk
    }


/// <summary>
/// アビリティーの対象となるための条件を表現できる
/// e.g. Atkが2以下のカード、コストが4以上のカード
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

    #region 制限を表現するパラメーター
    [SerializeField] private TargetEnum target;
    [SerializeField] private ConditionOperation conditionOperation;
    [SerializeField] private int threshold;
    [SerializeField] private Parameter targetParameter;
    [SerializeField] private bool attackHero;

    /// <summary>
    /// カードの所有者に関する制限
    /// </summary>
    public TargetEnum Target => target;

    /// <summary>
    /// ステータスに関する制限を表現する演算子
    /// e.g. Atkが2 "以下"
    /// </summary>
    public ConditionOperation ConditionOperation => conditionOperation;

    /// <summary>
    /// どのステータスに関数制限を加えるか 
    ///  e.g. "Atk" が2以下
    /// </summary>
    public Parameter TargetParameter => targetParameter;

    /// <summary>
    /// ステータスに関する制限のthreshold
    /// e.g. Atkが "2" 以下
    /// </summary>
    public int Threshold => threshold;

    /// <summary>
    /// ヒーローを選択することを許可するか
    /// (未実装)
    /// </summary>
    public bool AttackHero => attackHero;
    #endregion

    /// <summary>
    /// 引数のカードの中から、制限をクリアしているカードを探す
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="playerCards"></param>
    /// <param name="rivalCards"></param>
    /// <returns></returns>
    public List<StatusBase> DecideOption(StatusBase owner, List<StatusBase> playerCards, List<StatusBase> rivalCards)
    {
        //アビリティー使用者のカード
        List<StatusBase> ownCards = owner.IsPlayer ? playerCards : rivalCards;

        //アビリティー使用者にとって、相手のカード
        List<StatusBase> opponentCards = owner.IsPlayer ? rivalCards : playerCards;

        //まずはカードの所有者で絞った際に生き残る選択肢
        List<StatusBase> targetOption = null;

        //カードの所有者で絞る(e.g. 相手のカード, ランダムな自分のカード)
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
                Debug.LogError("この対象は未実装です");
                break;
        }


        //所有者で絞った際に生き残る選択肢を、さらに絞り込む

        //ステータスで絞った際に生き残る選択肢
        IEnumerable<StatusBase> ans = new List<StatusBase>();

        //演算子をデリゲート型で表現する (e.g. Atkが2"以下")
        Comparison<int> comparison = ConditionOperation switch
        {
            ConditionOperation.NoCondition => (int val, int threshold) => 1,
            ConditionOperation.Equal => (int val, int threshold) => val == threshold ? 1 : -1,
            ConditionOperation.LessOrEqual => (int val, int threshold) => val <= threshold ? 1 : -1,
            ConditionOperation.MoreOrEqual => (int val, int threshold) => val >= threshold ? 1 : -1,
            _ => throw new NotImplementedException()
        };

        //ステータスの条件を満たす選択肢を探す
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



