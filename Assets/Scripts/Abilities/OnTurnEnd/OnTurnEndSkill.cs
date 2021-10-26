using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnTurnEndSkill : Ability
{
    public override AbilityType AbilityType => AbilityType.OnTurnEnd;

    //public Condition Condition => new Condition(TargetEnum.Owner, ConditionOperation.NoCondition, -1, Parameter.Atk);
}
