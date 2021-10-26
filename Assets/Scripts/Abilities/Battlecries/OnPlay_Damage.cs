using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Battlecry/Damage")]
public class OnPlay_Damage : OnPlayAbility
{
    [SerializeField] private int damage;

    public override string SkillName => "ダメージ";

    public override CardType[] TargetType => new CardType[1] { CardType.Chara };

    protected override float CalculateScore(StatusBase owner, StatusBase target)
    {
        //攻撃対象がないのにこのカードの使うのはナンセンス(マイナス評価)
        int ans = -damage;

        if (target is Status_Chara chara)
        {
            if (chara.Hp <= damage)
            {
                ans = chara.Hp + chara.Atk * 2;
            }
            else
            {
                ans = -1;
            }
        }
        else if(target is Status_Hero hero)
        {
            if (hero.Hp <= damage)
            {
                ans += (int)1e8;
            }
        }


        return ans;
    }

    protected override int[] RunAbility(StatusBase owner,StatusBase targets, int[] indices)
    {
        if (targets == null) return null;

        ((Status)targets).CardData.DamageHp(damage);

        return null;
    }
}
