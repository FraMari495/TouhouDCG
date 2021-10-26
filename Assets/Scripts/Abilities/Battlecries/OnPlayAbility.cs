using Position;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class OnPlayAbility: AbilityBase
{
    /// <summary>
    /// アビリティーのターゲットを選択できるか否か
    /// </summary>
    public bool TargetRequired => Condition.Target != TargetEnum.NoTarget;

    /// <summary>
    /// プレイ時アビリティーの場合、プレイヤーが対称を指定することができる
    /// </summary>
    [SerializeField] private Condition condition;

    /// <summary>
    /// 対称となりうるカードの条件
    /// </summary>
    public Condition Condition => condition;

    /// <summary>
    /// このアビリティーはプレイ時に発動する
    /// </summary>
    public override AbilityType AbilityType => AbilityType.OnPlay;

    /// <summary>
    /// Skill()の引数に入るカードのタイプ
    /// </summary>
    public abstract CardType[] TargetType { get; }

    /// <summary>
    /// アビリティーを発動する前に、対称を選択する
    /// </summary>
    /// <param name="owner"></param>
    /// <returns></returns>
    public int[] Run(StatusBase owner,StatusBase Target, int[] indices) =>RunAbility(owner, Target,indices);

    /// <summary>
    /// 具体的なアビリティーの中身
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="targets"></param>
    protected abstract int[] RunAbility(StatusBase owner, StatusBase targets, int[] indices);

    /// <summary>
    /// for AI
    /// このアビリティーを使用して最大スコアを得る方法
    /// (ターゲットを誰にすべきか & スコアの取得)
    /// </summary>
    /// <param name="owner"></param>
    /// <returns></returns>
    public PredictedScore Predict(StatusBase owner)
    {

        if (condition.Target != TargetEnum.NoTarget)
        {
            //選択肢が与えられるようなアビリティーの場合

            #region 選択可能なカードを決定する
            //フィールド上の、プレイヤーのカード全て
            var playerCards = Field.I(true).GetStatus(TargetType);

            //フィールド上の、相手のカード全て
            var rivalCards = Field.I(false).GetStatus(TargetType);

            //選択可能なカード全て
            var options = condition.DecideOption(owner, playerCards, rivalCards);
            #endregion

            #region スコアの最大化

            float maxScore = -(int)1e8;
            StatusBase target = null;

            //すべての選択肢を試す
            foreach (var item in options)
            {
                //スコアを計算し
                float score = CalculateScore(owner, item);

                //最大値を更新
                if (maxScore < score)
                {
                    target = item;
                    maxScore = score;
                }
            }
            #endregion

            return new PredictedScore(target, maxScore);
        }
        else
        {
            //選択肢のないアビリティーの場合
            return new PredictedScore(null, CalculateScore(owner, null));
        }
    }

    /// <summary>
    /// for AI
    /// アビリティーの評価を計算
    /// </summary>
    /// <param name="owner">アビリティーの使用者</param>
    /// <param name="target">アビティーターゲット</param>
    /// <returns></returns>
    protected abstract float CalculateScore(StatusBase owner,StatusBase target);


}


/// <summary>
/// AIの評価のまとめ
/// このスキルを発動した場合、獲得するスコアが最大とするようなターゲットとそのスコア
/// </summary>
public class PredictedScore
{
    public PredictedScore(StatusBase target,float score)
    {
        Target = target;
        Score = score;
    }

    /// <summary>
    /// スコアが最大化されるターゲット
    /// </summary>
    public StatusBase Target { get; }

    /// <summary>
    /// 最大化されたスコア
    /// </summary>
    public float Score { get; } = 0;

}