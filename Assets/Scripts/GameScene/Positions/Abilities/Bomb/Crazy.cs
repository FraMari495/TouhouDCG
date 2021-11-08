using Position;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Bomb/Crazy")]
internal class Crazy : BombAbilityBase
{
    public override string SkillName => "l‚ğ‹¶‚í‚·’ö“x‚Ì”\—Í";

    public override float CalculateScore(StatusBase owner)
    {
        var cards = Field.I(!owner.IsPlayer).Cards.ConvertType<Status_Chara>().NonNull().ToList();

        if (cards.Count < 2) return -(int)1e8;

        List<int> scoreOptions = new List<int>();

        for (int i = 0; i < cards.Count; i++)
        {
            for (int j = i+1; j < cards.Count; j++)
            {
                int temp = 0;
                Status_Chara chara1 = cards[i];
                Status_Chara chara2 = cards[j];

                if (chara1.Atk >= chara2.Hp || chara1.CharaData.MyAbilities.HaveSpecialStatus(SpecialStatus.Killer)) temp += chara2.Atk * 3;
                if (chara2.Atk >= chara1.Hp || chara2.CharaData.MyAbilities.HaveSpecialStatus(SpecialStatus.Killer)) temp += chara1.Atk * 3;

                scoreOptions.Add(temp);
            }
        }

        return (float)scoreOptions.Average();
    }

    protected override int[] RunAbility(StatusBase owner, int[] indices)
    {
        var cards = Field.I(!owner.IsPlayer).Cards.ConvertType<Status_Chara>().NonNull().ToList();

        if (cards.Count < 2) return null;

        if(indices == null)
        {
            indices = new int[cards.Count];
            for (int i = 0; i < cards.Count; i++)
            {
                indices[i] = i;
            }
            indices = indices.Shuffle().ToArray();
        }


        Status_Chara attacker = cards[indices[0]];
            Status_Chara target = cards[indices[1]];

        Field.I(!owner.IsPlayer).Attack(attacker.PlayableCardId, target.PlayableCardId);
        return indices;
    }
}
