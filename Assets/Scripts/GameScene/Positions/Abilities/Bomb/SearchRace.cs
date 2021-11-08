using Position;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Bomb/SearchRace")]
internal class SearchRace : BombAbilityBase
{
    [SerializeField] private Race targetRace;

    public override string SkillName => "ƒT[ƒ`";

    public override float CalculateScore(StatusBase owner)
    {
        return 5;
    }

    protected override int[] RunAbility(StatusBase owner, int[] indices)
    {
        var options = Deck.I(owner.IsPlayer).GetCardsOfRace(targetRace);

        int index = -1;

        if(indices == null)
        {
            index = Random.Range(0, options.Count);
        }
        else
        {
            index = indices[0];
        }

        if (options.Count > 0)
        {
            Deck.I(owner.IsPlayer).Draw(options[index]);
        }
        return new int[1] { index };
    }
}
