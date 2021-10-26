using Position;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Bomb/PerfectFreeze")]
public class ParfectFreeze : BombAbilityBase
{
    public override string SkillName => "パーフェクトフリーズ";

    public override float CalculateScore(StatusBase owner)
    {
        return Field.I(!owner.IsPlayer).Cards.ConvertType<Status_Chara>().NonNull().ConvertAll(c => (int)c.CharaData.AtkData).ToList().Sum();
    }

    protected override int[] RunAbility(StatusBase owner, int[] indices)
    {
        Field.I(!owner.IsPlayer).Cards.ConvertType<Status_Chara>().NonNull().ForEach(c => c.AddEffect(StatusEffect.Freeze));
        return null;
    }
}
