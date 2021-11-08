using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 封印アビリティー
/// カードが持つ特殊能力(RunningAbilityクラスの中身)を全て消す
/// </summary>
[CreateAssetMenu(menuName = "Skill/Battlecry/Seal")]
internal class OnPlay_Seal : OnPlayAbility
{
    /// <summary>
    /// アビリティーの対象はキャラクターカード
    /// </summary>
    public override CardType[] TargetType => new CardType[1] { CardType.Chara };

    public override string SkillName => "封印";

    protected override float CalculateScore(StatusBase owner, StatusBase target)
    {
        float ans = -6;
        Debug.LogWarning("特殊攻撃や特殊体力は、列挙型にすべき");
        if(target is Status_Chara chara)
        {
            ans = Status_Chara.RunningAbilities.GetAbilityScore(chara);
        }
        return ans;
    }

    protected override int[] RunAbility(StatusBase owner, StatusBase targets, int[] indices)
    {
        if (targets == null) return null;
        ((Status_Chara)targets).AddEffect(StatusEffect.Sealed);
        return null;
    }
}
