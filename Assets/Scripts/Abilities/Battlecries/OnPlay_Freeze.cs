using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Battlecry/Freeze")]
public class OnPlay_Freeze : OnPlayAbility
{
    /// <summary>
    /// �A�r���e�B�[�̑Ώۂ̓L�����N�^�[�J�[�h
    /// </summary>
    public override CardType[] TargetType => new CardType[1] { CardType.Chara };

    public override string SkillName => "����";

    protected override float CalculateScore(StatusBase owner, StatusBase target)
    {
        return (target is Status_Chara chara) ? chara.Atk / 2f + chara.Hp / 3f : -3;
    }

    protected override int[] RunAbility(StatusBase owner, StatusBase targets, int[] indices)
    {
        if (targets is Status_Chara chara)
        {
            chara.AddEffect(StatusEffect.Freeze);
        }
        return null;
    }

}
