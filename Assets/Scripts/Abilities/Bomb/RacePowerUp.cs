using Position;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Bomb/RacePowerUp")]
public class RacePowerUp : BombAbilityBase
{
    [SerializeField] private Race targetRace;
    [SerializeField] private int atkUp;
    [SerializeField] private int hpUp;

    public override string SkillName => "種族パワーアップ";

    public override float CalculateScore(StatusBase owner)
    {
        int targetNum = Field.I(owner.IsPlayer).GetCardsOfRace(targetRace).Count;
        return targetNum * (hpUp + atkUp * 2);
    }

    protected override int[] RunAbility(StatusBase owner, int[] indices)
    {
        var targets = Field.I(owner.IsPlayer).GetCardsOfRace(targetRace);

        foreach (var target in targets)
        {
            target.AddAtk(atkUp);
            target.AddHp(hpUp);
        }

        return null;

    }
}
