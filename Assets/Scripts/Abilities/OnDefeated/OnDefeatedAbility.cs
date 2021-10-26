using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnDefeatedAbility : Ability
{
    public override AbilityType AbilityType => AbilityType.OnDefeated;

    //public Condition Condition => new Condition(TargetEnum.Owner, ConditionOperation.NoCondition, -1, Parameter.Atk);
}
