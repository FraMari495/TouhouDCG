using Position;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Bomb/FourOfAkind")]
internal class FourOfAkind : BombAbilityBase
{
    [SerializeField] private CardBook_Chara frandreCopy;
    public override string SkillName => "フォーオブアカインド";

    public override float CalculateScore(StatusBase owner)
    {
        return 18;
    }

    protected override int[] RunAbility(StatusBase owner,int[] indices)
    {

        int ownerPosition = Field.I(owner.IsPlayer).GetIndex(owner);
        IPlayable playable = frandreCopy.MakeCardToField(owner.IsPlayer);
        Field.I(playable.IsPlayer).SpecialSummon(ownerPosition, playable);


        for (int i = 0; i < 2; i++)
        {
            ownerPosition = Field.I(owner.IsPlayer).GetIndex(owner);
            playable = frandreCopy.MakeCardToField(owner.IsPlayer);
            Field.I(playable.IsPlayer).SpecialSummon(ownerPosition + 1, playable);
        }
        return null;
    }
}
