using Position;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Bomb/AllDamage")]
internal class Bomb_AllDamage : BombAbilityBase
{
    [SerializeField] private int damageMax;
    [SerializeField] private int damageMin;

    public override string SkillName => "全体ダメージ";

    public override float CalculateScore(StatusBase owner)
    {
        float damage_ave = (damageMax + damageMin) / 2f;

        float score = -2;

        foreach (var item in Field.I(!owner.IsPlayer).Cards.ConvertType<Status_Chara>().NonNull())
        {
            score+=2;
            score += (item.Hp + item.CharaData.MyAbilities.Diffense - damage_ave > 0) ? 0 : item.Atk * 4;
        }

        return score;
    }

    protected override int[] RunAbility(StatusBase owner, int[] indices)
    {
        var cards = Field.I(!owner.IsPlayer).Cards.ConvertType<Status_Chara>().NonNull().ConvertAll(c=>c.CharaData);
        if (indices == null) {
            indices = new int[cards.Count()];
            indices = Array.ConvertAll(indices, n => UnityEngine.Random.Range(damageMin, damageMax + 1));
        }

        for (int i = 0; i < cards.Count(); i++)
        {
            cards.ElementAt(i).DamageHp(indices[i]);
        }

        return indices;
    }
}
