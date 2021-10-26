using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Battlecry/Destroy")]
public class OnPlaySkill_Destroy : OnPlayAbility
{
    /// <summary>
    /// �A�r���e�B�[�̑Ώۂ̓L�����N�^�[�J�[�h
    /// </summary>
    public override CardType[] TargetType => new CardType[1] { CardType.Chara };

    public override string SkillName => "�j��";

    protected override float CalculateScore(StatusBase owner, StatusBase target)
    {
        float ans = 0;
        if(target is Status_Chara chara)
        {
            ans += chara.Atk * 2 + chara.Hp;
        }
        return ans;
    }

    protected override int[] RunAbility(StatusBase owner, StatusBase targets, int[] indices)
    {
        if (targets is Status_Chara chara)
        {
            chara.AddEffect(StatusEffect.Dead);
        }

        return null;
    }
}
