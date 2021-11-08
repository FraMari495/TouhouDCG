using Position;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Battlecry/Draw")]
internal class OnPlay_Draw : OnPlayAbility
{
    public override CardType[] TargetType => null;

    public override string SkillName => "プレイ時ドロー";

    protected override float CalculateScore(StatusBase owner,StatusBase target)
    {
        if (Hand.I(owner.IsPlayer).Cards.Count<3)
        {
            return 3;
        }
        else
        {
            return 1;
        }
    }

    protected override int[] RunAbility(StatusBase owner, StatusBase targets, int[] indices)
    {
        Deck.I(owner.IsPlayer).Draw();

        return null;
    }
}
