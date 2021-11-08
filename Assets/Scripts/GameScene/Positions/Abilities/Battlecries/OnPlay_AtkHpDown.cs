using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Battlecry/AtkHpDown")]
internal class OnPlay_AtkHpDown : OnPlayAbility
{
    [SerializeField] private int atkDown;
    [SerializeField] private int hpDown;

    public override CardType[] TargetType => new CardType[1] { CardType.Chara };

    public override string SkillName => "ステータス低下";

    protected override float CalculateScore(StatusBase owner,StatusBase target)
    {
        int ans = (atkDown+hpDown)*-1;
        if(target is Status_Chara chara)
        {
            if (chara.Hp <= hpDown)
            {
                ans = chara.Hp + chara.Atk * 2;
            }
            else if(chara.Atk<=atkDown)
            {
                ans = chara.Atk * 2;
            }
            else
            {
                ans = atkDown * 2;
            }
        }
        return ans;
    }

    protected override int[] RunAbility(StatusBase owner, StatusBase targets, int[] indices)
    {
        if (targets is Status_Chara chara)
        {
            chara.DownHp(hpDown);
            chara.DownAtk(atkDown);

        }

        return null;
    }
}
