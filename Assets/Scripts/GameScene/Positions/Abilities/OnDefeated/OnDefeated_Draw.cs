using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Position;

[CreateAssetMenu(menuName = "Skill/OnDefeated/Draw")]
internal class OnDefeated_Draw : OnDefeatedAbility
{
    [SerializeField]private int number = 1;
    public override string SkillName => $"{number}ƒhƒ[";

    protected override int[] RunAbility(StatusBase targets,int[] indices)
    {
        Deck.I(targets.IsPlayer).Draw();
        return null;
    }
}
