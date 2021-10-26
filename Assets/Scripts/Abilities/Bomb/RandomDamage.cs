using Position;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Bomb/RandomDamage")]
public class RandomDamage : BombAbilityBase
{
    [SerializeField] private int damage;

    public override string SkillName => "ランダムダメージ";

    public override float CalculateScore(StatusBase owner)
    {
        double hpAve = Field.I(!owner.IsPlayer).Cards.ConvertType<Status_Chara>().NonNull().ConvertAll(c => c.Hp).ToList().Average();
        double atkAve = Field.I(!owner.IsPlayer).Cards.ConvertType<Status_Chara>().NonNull().ConvertAll(c => c.Atk).ToList().Average();

        return hpAve < damage ? (int)atkAve * 3 : 0;
    }

    protected override int[] RunAbility(StatusBase owner, int[] indices)
    {
        var cards = Field.I(!owner.IsPlayer).Cards.ConvertType<Status_Chara>().NonNull().ToList();
        int rnd = indices != null? indices[0]: Random.Range(0, cards.Count);
        cards[rnd].CharaData.DamageHp(damage);
        return new int[1] { rnd };
    }
}
